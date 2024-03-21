using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    Player player;

    ItemDataManager itemDataManager;

    public ItemDataManager ItemData => itemDataManager;

    InventoryUI inventoryUI;

    public InventoryUI InventoryUI => inventoryUI;

    protected override void OnPreInitialize()
    {
        base.OnPreInitialize();
        itemDataManager = GetComponent<ItemDataManager>();
    }

    public Player Player
    {
        get
        {
            if(player == null)
            {
                player = FindAnyObjectByType<Player>();
            }
            return player;
        }
    }

    protected override void OnInitialize()
    {
        player = FindAnyObjectByType<Player>();
        inventoryUI = FindAnyObjectByType<InventoryUI>(); // FindAnyObjectByType이 제일 빠름
    }
}
