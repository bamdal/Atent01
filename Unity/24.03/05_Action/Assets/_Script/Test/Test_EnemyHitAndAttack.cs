using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_EnemyHitAndAttack : TestBase
{
    // Start is called before the first frame update
    Enemy enemy;
    void Start()
    {
        Player player = GameManager.Instance.Player;
        player.Inventory.AddItem(ItemCode.IronSword);
        player.Inventory[0].EquipItem(player.gameObject);
        enemy = FindAnyObjectByType<Enemy>();
    }
#if UNITY_EDITOR

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        enemy.HP = 0;
    }

#endif
}
