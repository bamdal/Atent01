
using UnityEngine;

public class Factory : Singleton<Factory>
{
    BulletHolePool bulletHolePool;
    ItemAssaultRiflePool itemAssaultRiflePool;
    ItemShotGunPool itemShotGunPool;
    ItemHealPackPool itemHealPackPool;
    protected override void OnInitialize()
    {
        base.OnInitialize();
        bulletHolePool = GetComponentInChildren<BulletHolePool>();
        bulletHolePool?.Initialize();

        itemAssaultRiflePool = GetComponentInChildren<ItemAssaultRiflePool>();
        itemAssaultRiflePool?.Initialize();

        itemShotGunPool = GetComponentInChildren<ItemShotGunPool>();
        itemShotGunPool?.Initialize();

        itemHealPackPool = GetComponentInChildren<ItemHealPackPool>();
        itemHealPackPool?.Initialize();
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

    public GunItem GetAssaultRifle(Vector3 position)
    {
        GunItem item = itemAssaultRiflePool?.GetObject();
        item.transform.position = position;
        return item;
    }
    public GunItem GetShotGun(Vector3 position)
    {
        GunItem item = itemShotGunPool?.GetObject();
        item.transform.position = position;
        return item;
    }

    public HealItem GetHealPack(Vector3 position)
    {
        HealItem item = itemHealPackPool?.GetObject();
        item.transform.position = position;
        return item;
    }

}

