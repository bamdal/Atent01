using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Inventory : TestBase
{
    public ItemCode code = ItemCode.Ruby;
    [Range(0,5)]
    public uint index = 0;
    Inventory inven;
#if UNITY_EDITOR
    private void Start()
    {
        inven = new Inventory(null);
        inven.AddItem(ItemCode.Ruby);
        inven.AddItem(ItemCode.Sapphire);
        inven.AddItem(ItemCode.Sapphire);
        inven.AddItem(ItemCode.Emerald);
        inven.AddItem(ItemCode.Sapphire);
        inven.AddItem(ItemCode.Emerald);
        inven.MoveItem(1, 2);
        inven.Test_InventoryPrint();

    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        inven.AddItem(code);
        inven.Test_InventoryPrint();
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        inven.AddItem(code,index);
        inven.Test_InventoryPrint();
    }
    protected override void OnTest3(InputAction.CallbackContext context)
    {
        inven.RemoveItem(index);
        inven.Test_InventoryPrint();
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        inven.ClearSlot(index);
        inven.Test_InventoryPrint();
    }
    protected override void OnTest5(InputAction.CallbackContext context)
    {
        inven.ClearInventory();
        inven.Test_InventoryPrint();
    }
#endif
}
