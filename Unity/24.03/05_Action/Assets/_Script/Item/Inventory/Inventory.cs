using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 인벤토리 개념 (메모리상에서만 존재, UI 없음)
public class Inventory
{
    /// <summary>
    /// 인벤토리 기본 슬롯 개수(6칸)
    /// </summary>
    const int Default_Inventory_Size = 6;

    /// <summary>
    /// 인벤토리의 슬롯들
    /// </summary>
    InvenSlot[] slots;

    /// <summary>
    /// 인벤토리 슬롯에 접근하기 위한 인덱서
    /// </summary>
    /// <param name="index">슬롯의 인덱스</param>
    /// <returns>슬롯</returns>
    public InvenSlot this[uint index] => slots[index];
    //public InvenSlot this[uint index] => (index != tempSlotIndex) ? slots[index] : tempSlot;  // 이렇게도 가능

    /// <summary>
    /// 인벤토리 슬롯의 개수
    /// </summary>
    int SlotCount => slots.Length;

    /// <summary>
    /// 임시슬롯(드래그나 아이템 분리 작업용)
    /// </summary>
    InvenTempSlot tempSlot;

    /// <summary>
    /// 임시슬롯의 인덱스
    /// </summary>
    uint tempSlotIndex = 999999;

    /// <summary>
    /// 임시슬롯 프로퍼티
    /// </summary>
    public InvenTempSlot TempSlot => tempSlot;

    /// <summary>
    /// 아이템 데이터 매니저(아이템의 모든 종류별 정보를 가짐)
    /// </summary>
    ItemDataManager itemDataManager;

    /// <summary>
    /// 인벤토리를 가지고 있는 소유자
    /// </summary>
    Player owner;

    /// <summary>
    /// 소유자 확인용 프로퍼티
    /// </summary>
    public Player Owner => owner;

    /// <summary>
    /// 인벤토리 생성자
    /// </summary>
    /// <param name="owner">인벤토리의 소유자</param>
    /// <param name="size">인벤토리의 슬롯개수</param>
    public Inventory(Player owner, uint size = Default_Inventory_Size)
    {
        slots = new InvenSlot[size];
        for (uint i = 0; i < size; i++)
        {
            slots[i] = new InvenSlot(i);
        }
        tempSlot = new InvenTempSlot(tempSlotIndex);
        itemDataManager = GameManager.Instance.ItemData;    // 타이밍 조심 start에서 사용
        this.owner = owner;
    }

    /// <summary>
    /// 인벤토리에 특정아이템을 1개 추가하는 함수
    /// </summary>
    /// <param name="code">추가할 아이템의 코드</param>
    /// <returns>true면 추가 성공, false면 추가 실패</returns>
    public bool AddItem(ItemCode code)
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (AddItem(code, (uint)i))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 인벤토리에 특정 슬롯에 특정아이템을 1개 추가하는 함수
    /// </summary>
    /// <param name="code">추가할 아이템의 코드</param>
    /// <param name="slotIndex">아이템을 추가할 슬롯의 인덱스</param>
    /// <returns>true면 추가 성공, false면 추가 실패</returns>
    public bool AddItem(ItemCode code, uint slotIndex)
    {
        bool result = false;

        if (IsValidIndex(slotIndex))    // 인덱스가 적절한지 확인
        {
            // 적절한 인덱스면
            ItemData data = itemDataManager[code];  // 데이터가져오기
            InvenSlot slot = slots[slotIndex];      // 슬롯 가져오기
            if (slot.IsEmpty)   // 슬롯이 비었다면
            {
                slot.AssignSlotItem(data);  // 그대로 아이템 설정
                result = true;
            }
            else                // 슬롯이 안비었다면
            {
                if (slot.ItemData == data)  // 같은 종류에 아이템이 들어있다면
                {
                    result = slot.IncreaseSlotItem(out _);  // 아이템개수 증가 시도
                }
                else
                {
                    //Debug.Log($"아이템 추가 실패 : [{slotIndex}]번 슬롯에 다른 아이템이 들어있다");  // 다른 종류에 아이템이 이미들어있다
                }
            }
        }
        else
        {
            // 인덱스가 잘못들어왔다
            //Debug.Log($"아이템 추가 실패 : [{slotIndex}]는 잘못된 인덱스");
        }

        return result;
    }

    /// <summary>
    /// 인벤토리의 특정 슬롯에서 아이템을 일정 개수만큼 제거하는 함수
    /// </summary>
    /// <param name="slotIndex">아이템을 감소시킬 슬롯의 인덱스</param>
    /// <param name="decreaseCount">감소시킬 개수</param>
    public void RemoveItem(uint slotIndex, uint decreaseCount = 1)
    {
        if (IsValidIndex(slotIndex))
        {
            InvenSlot slot = slots[slotIndex];
            slot.DecreaseSlotItem(decreaseCount);
        }
        else
        {
            Debug.Log($"슬롯 아이템 감소 실패 : [{slotIndex}]는 없는 인덱스");
        }
    }

    /// <summary>
    /// 인벤토리의 특정 슬롯을 비우는 함수
    /// </summary>
    /// <param name="slotIndex">아이템을 비울 슬롯</param>
    public void ClearSlot(uint slotIndex)
    {
        if (IsValidIndex(slotIndex))
        {
            InvenSlot slot = slots[slotIndex];
            slot.ClearSlotItem();
        }
        else
        {
            Debug.Log($"아이템 삭제실패 : [{slotIndex}]는 없는 인덱스");
        }
    }

    /// <summary>
    /// 인벤토리의 슬롯을 전부 비우는 함수
    /// </summary>
    public void ClearInventory()
    {
        foreach (var slot in slots)
        {
            slot.ClearSlotItem();
        }
    }

    /// <summary>
    /// 인벤토리의 from 슬롯에 있는 아이템을 to 위치로 옮기는 함수
    /// </summary>
    /// <param name="from">위치 변경 시작 인덱스</param>
    /// <param name="to">위치 변경 도착 인덱스</param>
    public void MoveItem(uint from, uint to)
    {
        // from과 to는 서로 다른 지점이고 모두 vaild한 인덱스 여야한다
        if ((from != to) && IsValidIndex(from) && IsValidIndex(to))
        {
            bool fromIsTemp = (from == tempSlotIndex);
            InvenSlot fromSlot = (from == tempSlotIndex) ? TempSlot : slots[from];  // from 슬롯 가져오기


            if (!fromSlot.IsEmpty)
            {
                // from에 아이템이 있다
                bool toIstemp = (to == tempSlotIndex);
                InvenSlot toSlot = null;
                if (toIstemp)
                {
                    // to가 temp슬롯이면
                    toSlot = TempSlot;                      // 슬롯저장하고
                    TempSlot.SetFromIndex(fromSlot.Index);  // 드래그 시작 인덱스 저장(fromIndex)
                }
                else
                {
                    toSlot = slots[to];                     // temp슬롯이 아니면 슬롯만 저장
                }

                if (fromSlot.ItemData == toSlot.ItemData)
                {
                    // 같은 종류의 아이템 => 채울 수 있는데 까지 채우고 남은것은 from이 가지고 있는다
                    toSlot.IncreaseSlotItem(out uint overCount, fromSlot.ItemCount);    // from이 가진 개수만큼 to 에 추가
                    //fromSlot.AssignSlotItem(fromSlot.ItemData, overCount);
                    fromSlot.DecreaseSlotItem(fromSlot.ItemCount - overCount);          // from에서 to로 넘어간 개수만큼만 감소

                    //Debug.Log($"[{from}]번 슬롯에서 [{to}]번 슬롯으로 아이템 합치기");
                }
                else
                {
                    // 다른 종류의 아이템(or 비어있다) => 서로 스왑
                    if (fromIsTemp)
                    {
                        // temp슬롯에서 to 슬롯으로 옮기는 경우
                        //      returnSlot(temp가 기록해 놓은 FromIndex 슬롯)에 toSlot 슬롯의 데이터 넣기
                        //      toSlot에 TempSlot의 데이터 넣기

                        // slots[TempSlot.FromIndex]
                        //  비어있다 => 기존대로 처리
                        //  들어있다.
                        //   - 다른 종류의 아이템 => 교환 or 안함
                        //   - 같은 종류의 아이템 => 합치기

                        InvenSlot returnSlot = slots[TempSlot.FromIndex];   // temp가 기록해 놓은 FromIndex에 있는 슬롯

                        if (returnSlot.IsEmpty)
                        {
                            // returnSlot이 비어있는 경우(to->return, temp->to, temp 비우기)
                            returnSlot.AssignSlotItem(toSlot.ItemData, toSlot.ItemCount, false);
                            toSlot.AssignSlotItem(TempSlot.ItemData, TempSlot.ItemCount, false);
                            TempSlot.ClearSlotItem();
                        }
                        else
                        {
                            // returnSlot에 아이템이 있다.
                            if (TempSlot.ItemData == toSlot.ItemData)
                            {
                                // 같은 종류의 아이템이다. => toSlot의 내용을 tempSlot에 합치기 시도
                                TempSlot.IncreaseSlotItem(out uint overCount, toSlot.ItemCount);
                                toSlot.DecreaseSlotItem(toSlot.ItemCount - overCount);
                                // 아이템이 남아도 아래쪽에서 처리
                            }
                            // tempSlot과 toSlot을 스왑
                            SwapSlot(tempSlot, toSlot);
                        }
                    }
                    else
                    {
                        // fromSlot이 TempSlot이 아닌 경우
                        SwapSlot(fromSlot, toSlot);
                    }
                    /*                    // 다른 종류의 아이템, 비어있다
                                        ItemData tempData = fromSlot.ItemData;
                                        uint tempCount = fromSlot.ItemCount;
                                        bool tempEquip = fromSlot.IsEquipped;

                                        //Debug.Log($"[{from}]번 슬롯에서 [{to}]번 슬롯의 아이템 서로 교체");

                                        fromSlot.AssignSlotItem(toSlot.ItemData, toSlot.ItemCount, toSlot.IsEquipped);
                                        toSlot.AssignSlotItem(tempData, tempCount, tempEquip);*/

                }

            }

        }
        else
        {

        }

    }

    /// <summary>
    /// 슬롯 스왑용 함수
    /// </summary>
    /// <param name="slotA"></param>
    /// <param name="slotB"></param>
    void SwapSlot(InvenSlot slotA, InvenSlot slotB)
    {
        ItemData tempData = slotA.ItemData;
        uint tempCount = slotA.ItemCount;

        slotA.AssignSlotItem(slotB.ItemData, slotB.ItemCount, false);
        slotB.AssignSlotItem(tempData, tempCount, false);
    }


    /// <summary>
    /// 인벤토리의 특정 슬롯에서 아이템을 일정량 덜어내어 임시 슬롯으로 보내는 함수
    /// </summary>
    /// <param name="slotIndex">아이템을 덜어낼 슬롯</param>
    /// <param name="count">덜어낼 개수</param>
    public void SplitItem(uint slotIndex, uint count)
    {
        if (IsValidIndex(slotIndex))
        {
            InvenSlot slot = slots[slotIndex];
            count = Math.Min(count, slot.ItemCount);    // count가 슬롯에 들어있는 개수보다 크면 들어있는 개수까지만

            TempSlot.AssignSlotItem(slot.ItemData, count);  // 임시 슬롯에 우선 넣고
            slot.DecreaseSlotItem(count);                   // 목적하던 슬롯에서 빼기

        }
    }

    /// <summary>
    /// 인벤토리를 정렬하는 함수
    /// </summary>
    /// <param name="sortBy">정렬 기준</param>
    /// <param name="isAcending">true면 오름차순, false면 내림차순</param>
    public void SlotSorting(ItemSortBy sortBy, bool isAcending = true)
    {
        // 정렬을 위한 임시 리스트
        List<InvenSlot> temp = new List<InvenSlot>(slots);  // slots를 기반으로 리스트 생성

        // 정렬 방법에 따라 임시 리스트 정렬
        switch (sortBy)
        {
            case ItemSortBy.Code:
                temp.Sort((current, other) =>     // x,y는 temp리스트에 들어있는 요소중 2개, X 기준으로 생각
                {
                    if (current.ItemData == null) // 비어있는 슬롯을 뒤쪽으로 보내기
                    {
                        return 1;
                    }
                    if (other.ItemData == null)  // other 보다 currnet가 클태니 무조건 앞으로 보내기 위해 -1
                    {
                        return -1;
                    }
                    if (isAcending)             // 오름차순/내림차순에 따라 처리
                    {

                        return current.ItemData.code.CompareTo(other.ItemData.code);
                    }
                    else
                    {

                        return other.ItemData.code.CompareTo(current.ItemData.code);
                    }
                });
                break;
            case ItemSortBy.Name:
                temp.Sort((current, other) =>
    {
        if (current.ItemData == null) // 비어있는 슬롯을 뒤쪽으로 보내기
        {
            return 1;
        }
        if (other.ItemData == null)  // other 보다 currnet가 클태니 무조건 앞으로 보내기 위해 -1
        {
            return -1;
        }
        if (isAcending)             // 오름차순/내림차순에 따라 처리
        {
            return current.ItemData.itemName.CompareTo(other.ItemData.itemName);
        }
        else
        {
            return other.ItemData.itemName.CompareTo(current.ItemData.itemName);
        }

    });
                break;
            case ItemSortBy.Price:
                temp.Sort((current, other) =>
                {
                    if (current.ItemData == null) // 비어있는 슬롯을 뒤쪽으로 보내기
                    {
                        return 1;
                    }
                    if (other.ItemData == null)  // other 보다 currnet가 클태니 무조건 앞으로 보내기 위해 -1
                    {
                        return -1;
                    }
                    if (isAcending)             // 오름차순/내림차순에 따라 처리
                    {
                        return current.ItemData.price.CompareTo(other.ItemData.price);
                    }
                    else
                    {
                        return other.ItemData.price.CompareTo(current.ItemData.price);
                    }

                });

                break;
        }

        // 임시 리스트의 내용을 슬롯에 설정
        List<(ItemData, uint, bool)> sortedData = new List<(ItemData, uint, bool)>(SlotCount);  // 튜플 사용
        foreach (var slot in temp)
        {
            sortedData.Add((slot.ItemData, slot.ItemCount, slot.IsEquipped));   // 필요 데이터만 복사해서 가지기
        }
        int index = 0;
        foreach (var data in sortedData)
        {
            slots[index].AssignSlotItem(data.Item1, data.Item2, data.Item3);    // 튜플 내용을 슬롯에 추가
            index++;
        }
    }

    /// <summary>
    /// 슬롯 인덱스가 적절한 인덱스인지 확인하는 함수
    /// </summary>
    /// <param name="index">확인할 함수</param>
    /// <returns>true면 적절한 인덱스, false면 잘못된 인덱스</returns>
    bool IsValidIndex(uint index)
    {
        return (index < SlotCount) || (index == tempSlotIndex);
    }

    const uint FailFind = uint.MaxValue;

    /// <summary>
    /// 비어있는 슬롯을 찾아 인덱스를 돌려주는 함수
    /// </summary>
    /// <param name="index">비어있는 슬롯을 찾았으면 그 슬롯의 인덱스</param>
    /// <returns>찾았으면 true, 못찾았으면 false</returns>
    public bool FindEmptySlot(out uint index)
    {
        bool result = false;
        index = FailFind;

        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                index = slot.Index;
                result = true;
                break;  // 하나라도 빈것이 나오면 루프 종료
            }
        }

        return result;
    }

#if UNITY_EDITOR
    public void Test_InventoryPrint()
    {
        // 출력예시
        // [루비 (1/10), 사파이어(2/3), (빈칸), 에메랄드(3/5),(빈칸),(빈칸)]
        string printText = "[";
        for (int i = 0; i < SlotCount - 1; i++)
        {
            if (!slots[i].IsEmpty)
            {
                InvenSlot slot = slots[i];
                printText += $"{slot.ItemData.itemName} ({slot.ItemCount}/{slot.ItemData.maxStackCount}), ";
            }
            else
            {
                printText += "(빈칸),";

            }



        }
        if (!slots[SlotCount - 1].IsEmpty)
        {
            InvenSlot slot = slots[SlotCount - 1];
            printText += $"{slot.ItemData.itemName} ({slot.ItemCount}/{slot.ItemData.maxStackCount}) ";
        }
        else
        {
            printText += "(빈칸)";

        }

        Debug.Log(printText);
    }
#endif
}
