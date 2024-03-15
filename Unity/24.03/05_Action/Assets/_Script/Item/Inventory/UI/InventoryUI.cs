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
        }

        TempSlotUI.InitializeSlot(inven.TempSlot);
    }
    private void OnItemMoveBegin(uint index)
    {
        inven.MoveItem(index, TempSlotUI.Index);
    }

    private void OnItemMoveEnd(uint index, bool isSuccess)
    {
        inven.MoveItem(TempSlotUI.Index, index);
    }


}
