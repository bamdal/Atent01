
using UnityEngine;

public class Factory : Singleton<Factory>
{
    BulletHolePool bulletHolePool;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        bulletHolePool = GetComponentInChildren<BulletHolePool>();
        bulletHolePool?.Initialize();
    }

    public BulletHole GetBulletHole()
    {
        return bulletHolePool?.GetObject();
    }

    public BulletHole GetBulletHole(Vector3 position, Vector3 normal, Vector3 reflect)
    {
        BulletHole bulletHole = bulletHolePool?.GetObject(position, normal);
        bulletHole.Initialize(position, normal, reflect);   
        return bulletHole;
    }
}

