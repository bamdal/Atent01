using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Factory : TestBase
{
    public PoolObjectType objectType;
    public Vector3 position = Vector3.zero;
    Player player;
    protected override void OnTest1(InputAction.CallbackContext context)
    {
        Factory.Instance.GetObject(objectType, position);
    }




    protected override void OnTest2(InputAction.CallbackContext context)
    {
        switch (objectType)
        {
            case PoolObjectType.PlayerBullet:
                player = FindAnyObjectByType<Player>();
                position = player.transform.position;
                Factory.Instance.GetBullet(position);
                break;
            case PoolObjectType.Hit:
                Factory.Instance.GetHitEffect(position);
                break;
            case PoolObjectType.ExplosionEffect:
                Factory.Instance.GetExplosionEffect(position);
                break;
            case PoolObjectType.Enemy:
                Factory.Instance.GetEnemy(position);
                break;
            default:
                break;
        }

    }
}
