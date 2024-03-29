using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_EnemyHitAndAttack : TestBase
{
    // Start is called before the first frame update
    void Start()
    {
        Player player = GameManager.Instance.Player;
        player.Inventory.AddItem(ItemCode.IronSword);
        player.Inventory[0].EquipItem(player.gameObject);
    }

}
