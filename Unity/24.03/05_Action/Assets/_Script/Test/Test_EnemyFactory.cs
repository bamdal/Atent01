using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_EnemyFactory : TestBase
{
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Factory.Instance.GetEnemy();
    }
    protected override void OnTest2(InputAction.CallbackContext context)
    {
        Factory.Instance.GetEnemy(1,Vector3.zero);
    }
    protected override void OnTest3(InputAction.CallbackContext context)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemies)
        {
            enemy.HP= 0;
        }
    }
}
