using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_InventoryUI : TestBase
{
    public InventoryUI inventotyUI;
    Inventory inven;

    public ItemCode code = ItemCode.Ruby;

    [Range(0, 5)]
    public uint index = 0;


    [Range(0, 5)]
    public uint fromIndex = 0;

    [Range(0, 5)]
    public uint toIndex = 0;

    public ItemSortBy sortBy = ItemSortBy.Code;
    public bool isAcending = true;
#if UNITY_EDITOR
    private void Start()
    {
        inven = new Inventory(null);
        inven.AddItem(ItemCode.Sapphire);
        inven.AddItem(ItemCode.Ruby);
        inven.AddItem(ItemCode.Sapphire);
        inven.AddItem(ItemCode.Emerald);
        inven.AddItem(ItemCode.Sapphire);
        inven.AddItem(ItemCode.Emerald);
        inven.MoveItem(1, 2);
        inven.Test_InventoryPrint();

        inventotyUI.InitializeInventory(inven);

    }


    protected override void OnTest1(InputAction.CallbackContext context)
    {
        inven.AddItem(code,index);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        inven.RemoveItem(index);
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        inven.MoveItem(fromIndex, toIndex);
    }
    protected override void OnTest4(InputAction.CallbackContext context)
    {
        inven.SlotSorting(sortBy, isAcending);
    }

    protected override void OnTest5(InputAction.CallbackContext context)
    {
        inven.ClearInventory();
    }
#endif
}
