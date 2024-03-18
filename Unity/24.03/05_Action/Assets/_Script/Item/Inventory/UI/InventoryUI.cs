using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        slotUIs = child.GetComponentsInChildren<InvenSlotUI>();

        TempSlotUI = GetComponentInChildren<TempSlotUI>();
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

        }

        TempSlotUI.InitializeSlot(inven.TempSlot);
    }



    private void OnItemMoveBegin(uint index)
    {
        inven.MoveItem(index, TempSlotUI.Index);    // 시작 -> 임시 아이템 옮기기
        TempSlotUI.Open();                          // 임시 슬롯 열기
    }

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
    }

    private void OnSlotClick(uint index)
    {
        if (!TempSlotUI.InvenSlot.IsEmpty)
        {
            OnItemMoveEnd(index, true); // 슬롯이 클릭되었을 때 실행되니 isSlotEnd는 true
        }
    }

}
