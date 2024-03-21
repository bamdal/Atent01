using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_ItemDrop : TestBase
{
    public InventoryUI inventoryUI;
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

    public Transform trans;
#if UNITY_EDITOR
    private void Start()
    {
        inven = new Inventory(GameManager.Instance.Player);
        inven.AddItem(ItemCode.Sapphire);
        inven.AddItem(ItemCode.Ruby);
        inven.AddItem(ItemCode.Sapphire);
        inven.AddItem(ItemCode.Emerald);
        inven.AddItem(ItemCode.Sapphire);
        inven.AddItem(ItemCode.Emerald);
        inven.MoveItem(1, 2);
        inven.Test_InventoryPrint();

        inventoryUI.InitializeInventory(inven);
        
    }
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Factory.Instance.MakeItem(code);
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        Factory.Instance.MakeItem(code, trans.position);
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        Factory.Instance.MakeItem(code, trans.position, true);
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        Factory.Instance.MakeItems(code, 10);
    }



    protected override void OnTest5(InputAction.CallbackContext context)
    {
        ItemObject[] items = FindObjectsByType<ItemObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (ItemObject item in items)
        {
            //ItemData data = item.Pickup();
            //Debug.Log(data);
        }
    }
#endif
}
