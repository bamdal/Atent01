using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PoolObjectType
{
    TurretBullet
}

public class Factory : Singleton<Factory>
{
    BulletPool bullet;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        bullet = GetComponentInChildren<BulletPool>(true);
        if(bullet != null)
            bullet.Initialize();
    }

    public GameObject GetObject(PoolObjectType type, Vector3? position = null, Vector3? euler = null)
    {
        GameObject result = null;
        switch (type)
        {
            case PoolObjectType.TurretBullet:
                result = bullet.GetObject(position, euler).gameObject;
                break;
            default:
                break;
        }
        return result;
    }

    public Bullet GetBullet()
    {
        return bullet.GetObject();
    }

    public Bullet GetBullet(Vector3 position, float angle = 0.0f)
    {
        return bullet.GetObject(position, angle * Vector3.forward); 
    }
}

// 팩토리 만들기
// - 이전 프로젝트 펙토리 이식
