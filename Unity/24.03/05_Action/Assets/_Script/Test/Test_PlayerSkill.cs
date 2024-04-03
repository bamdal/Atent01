using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_PlayerSkill : TestBase
{
    Player player;
    void Start()
    {
        player = GameManager.Instance.Player;
        player.Inventory.AddItem(ItemCode.IronSword);
        player.Inventory[0].EquipItem(player.gameObject);
        player.Inventory.AddItem(ItemCode.KiteShield);
        player.Inventory[1].EquipItem(player.gameObject);
    }

}
