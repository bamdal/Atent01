using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
