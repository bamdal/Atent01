using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    /// <summary>
    /// 인벤토리
    /// </summary>
    Inventory inven;

    /// <summary>
    /// 인벤토리의 슬롯들
    /// </summary>
    InvenSlotUI[] slotUIs;

    TempSlotUI TempSlotUI;

    /// <summary>
    /// 상세정보창
    /// </summary>
    DetailInfoUI detail;

    /// <summary>
    /// 아이템 분리창
    /// </summary>
    ItemDividerUI divider;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        slotUIs = child.GetComponentsInChildren<InvenSlotUI>();

        TempSlotUI = GetComponentInChildren<TempSlotUI>();
        child = transform.GetChild(2);
        detail = child.GetComponent<DetailInfoUI>();
        divider = GetComponentInChildren<ItemDividerUI>(true);

    }

    /// <summary>
    /// 인벤토리 초기화용 함수
    /// </summary>
    /// <param name="playerInventory">이 UI가 표시할 인벤토리</param>
    public void InitializeInventory(Inventory playerInventory)
    {
        inven = playerInventory;    // 저장

        for(uint i= 0; i<slotUIs.Length; i++)
        {
            slotUIs[i].InitializeSlot(inven[i]);    // 모든 슬롯 초기화
            slotUIs[i].onDragBegin += OnItemMoveBegin;
            slotUIs[i].onDragEnd += OnItemMoveEnd;
            slotUIs[i].onClick += OnSlotClick;
            slotUIs[i].onPointerEnter += OnItemDetailOn;
            slotUIs[i].onPointerExit += OnItemDetailOff;
            slotUIs[i].onPointerMove += OnSlotPointerMove;

        }

        TempSlotUI.InitializeSlot(inven.TempSlot);

        divider.onOkClick += OnDividerOK;
        divider.onCancelClick += OnDividerCanecel;
        divider.Close();
    }


    /// <summary>
    /// 드래그가 시작했을때 불리는 함수
    /// </summary>
    /// <param name="index"></param>
    private void OnItemMoveBegin(uint index)
    {
        detail.IsPause = true;
        inven.MoveItem(index, TempSlotUI.Index);    // 시작 -> 임시 아이템 옮기기
        TempSlotUI.Open();                          // 임시 슬롯 열기
    }

    /// <summary>
    /// 드래그가 종료 되었을 때 실행되는 함수
    /// </summary>
    /// <param name="index">슬롯에서 드래그 종료시 끝난 슬롯, 슬롯에서 종료를 안하면 시작한 슬롯 인덱스</param>
    /// <param name="isSlotEnd">드래그가 끝난지점이 슬롯이면 true 아니면 false</param>
    private void OnItemMoveEnd(uint index, bool isSlotEnd)
    {
        uint finalIndex = index;
        if(!isSlotEnd)
        {
            if (inven.FindEmptySlot(out uint emptySlotIndex))
            {
                finalIndex = emptySlotIndex;
            }
            else
            {
                // 바닥에 드랍
                Debug.LogWarning("바닥에 아이템 드랍해야 함");
                return;
            }
        }

        inven.MoveItem(TempSlotUI.Index,finalIndex);        // 임시 -> 도착으로 아이템 옮기기
        //inven.MoveItem(TempSlotUI.Index, TempSlotUI.FromIndex);

        if(TempSlotUI.InvenSlot.IsEmpty)
        {
            //TempSlotUI.Close(); // 임시 슬롯 닫기
        }
        detail.IsPause = false;
        if(isSlotEnd)
        {
            detail.Open(inven[index].ItemData);
        }
    }

    /// <summary>
    /// 슬롯을 클릭했을 때 실행되는 함수
    /// </summary>
    /// <param name="index">클릭한 슬롯의 인덱스</param>
    private void OnSlotClick(uint index)
    {
        if (TempSlotUI.InvenSlot.IsEmpty)
        {
            bool isShiftPress = (Keyboard.current.shiftKey.ReadValue() > 0);
            if(isShiftPress)
            {
                // 쉬프트가 눌려져 있으면 아이템 분리창 열기
                OnItemDividerOpen(index);

            }
            else
            {
                // 쉬프트가 안눌려진 상태면 아이템 사용 or 아이템 장비
            }
        }
        else 
        {
            OnItemMoveEnd(index, true); // 슬롯이 클릭되었을 때 실행되니 isSlotEnd는 true
          
        }
        

    }

    /// <summary>
    /// 아이템 상세 정보창을 여는 함수
    /// </summary>
    /// <param name="index">상세 정보창에서 표시될 아이템이 들어있는 슬롯</param>
    private void OnItemDetailOn(uint index)
    {
        detail.Open(slotUIs[index].InvenSlot.ItemData);
    }

    /// <summary>
    /// 아이템 상세 정보창을 닫는 함수
    /// </summary>
    private void OnItemDetailOff()
    {
        detail.Close();
    }

    /// <summary>
    /// 슬롯안에서 마우스 커서가 움직였을 때 실행되는 함수
    /// </summary>
    /// <param name="screen">마우스 커서의 스크린 좌표</param>
    private void OnSlotPointerMove(Vector2 screen)
    {
        detail.MovePosition(screen);
    }

    /// <summary>
    /// 아이템 분리창 열기
    /// </summary>
    /// <param name="index">아이템을 분리할 슬롯의 인덱스</param>
    void OnItemDividerOpen(uint index)
    {
        InvenSlotUI target = slotUIs[index];
        divider.transform.position = target.transform.position + Vector3.up* 100;
        if (divider.Open(target.InvenSlot))
        {
            detail.IsPause = true;

        }


    }

    /// <summary>
    /// 아이템 분리창의 OK버튼을 눌렀을 때 실행될 함수
    /// </summary>
    /// <param name="targetIndex">아이템을 나눌 슬롯</param>
    /// <param name="dividCount">나눌 개수</param>
    private void OnDividerOK(uint targetIndex, uint dividCount)
    {
        inven.DividItem(targetIndex, dividCount);
        TempSlotUI.Open();
    }

    /// <summary>
    /// 아이템 분리창에서 Cancel을 눌렀을때 실행될 함수
    /// </summary>
    private void OnDividerCanecel()
    {
        detail.IsPause = false;
    }



}
