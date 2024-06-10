using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_13_Enemy : TestBase
{
    // 적의 상태구현
    // 기즈모

    public Enemy enemy;
    public Transform respawn;

    private void Start()
    {
        if(enemy == null)
            enemy = FindAnyObjectByType<Enemy>();

        enemy.Respawn(respawn.position);

    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Vector3 pos = enemy.Test_GetRandomPosition();
        Debug.Log(pos);
    }

}
