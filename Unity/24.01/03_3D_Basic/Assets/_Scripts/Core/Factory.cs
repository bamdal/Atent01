using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PoolObjectType
{
    Bullet
}

public class Factory : Singleton<Factory>
{
    BulletPool bulletPool;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        bulletPool = GetComponentInChildren<BulletPool>(true);
        if(bulletPool != null)
            bulletPool.Initialize();
    }

    public GameObject GetObject(PoolObjectType type, Vector3? position = null, Vector3? euler = null)
    {
        GameObject result = null;
        switch (type)
        {
            case PoolObjectType.Bullet:
                result = bulletPool.GetObject(position, euler).gameObject;
                break;
            default:
                break;
        }
        return result;
    }

    public Bullet GetBullet()
    {
        return bulletPool.GetObject();
    }

    public Bullet GetBullet(Vector3 position, float angle = 0.0f)
    {
        return bulletPool.GetObject(position, angle * Vector3.forward); 
    }
}

// 팩토리 만들기
// - 이전 프로젝트 펙토리 이식
