using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_ObjectPool : TestBase
{
    // Start is called before the first frame update
    public BulletPool bulletPool;
    public HitPool hitPool;
    public WavePool enemyPool;
    public ExplosionPool explosionPool;

    private void Start()
    {
/*        pool.Initialize();
        hitPool.Initialize();
        enemyPool.Initialize();
        explosionPool.Initialize();*/
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Bullet bullet = bulletPool.GetObject();// 풀에서 오브젝트 하나 꺼내기
    }
    protected override void OnTest2(InputAction.CallbackContext context)
    {
        // hit
        HitEffect hitEffect = hitPool.GetObject();
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        // enemy
        Wave enemy = enemyPool.GetObject();
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        // explosion
        Explosion explosion = explosionPool.GetObject();
    }
}
