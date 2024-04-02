using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PoolObjectType
{
    Item,
    HitEffect,
    Slime,
    DamageText
}

public class Factory : Singleton<Factory>
{
    /// <summary>
    /// 노이즈 반지름
    /// </summary>
    public float noisePower = 0.5f;

    ItemPool itemPool;
    HitEffectPool hitEffectPool;
    EnemyPool enemyPool;
    DamageTextPool damageTextPool;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        enemyPool = GetComponentInChildren<EnemyPool>(true);
        if (enemyPool != null)
            enemyPool.Initialize();
        itemPool = GetComponentInChildren<ItemPool>(true);
        if (itemPool != null)
            itemPool.Initialize();
        hitEffectPool = GetComponentInChildren<HitEffectPool>(true);
        if (hitEffectPool != null)
            hitEffectPool.Initialize();
        damageTextPool = GetComponentInChildren<DamageTextPool>(true);
        if(damageTextPool != null)
            damageTextPool.Initialize();
    }

    public GameObject GetObject(PoolObjectType type, Vector3? position = null, Vector3? euler = null)
    {
        GameObject result = null;
        switch (type)
        {
            case PoolObjectType.Item:
                result = itemPool.GetObject(position, euler).gameObject;
                break;
            case PoolObjectType.HitEffect:
                result = hitEffectPool.GetObject(position, euler).gameObject;
                break;
            case PoolObjectType.DamageText:
                result = damageTextPool.GetObject(position, euler).gameObject;
                break;
            default:
                break;
        }
        return result;
    }

    /// <summary>
    /// 슬라임 하나 생성하는 함수
    /// </summary>
    /// <returns>슬라임 게임 오브젝트</returns>
    public Enemy GetEnemy()
    {
        return enemyPool.GetObject();
    }

    /// <summary>
    /// 슬라임 하나를 생성하는 함수
    /// </summary>
    /// <param name="position">소환 위치</param>
    /// <param name="angle">소환 각도</param>
    /// <returns>소환된 게임 오브젝트들</returns>
    public Enemy GetEnemy(Vector3 position, float angle = 0.0f)
    {
        return enemyPool.GetObject(position, angle * Vector3.forward);
    }

    /// <summary>
    /// 슬라임 하나를 특정 웨이포인트를 사용하고 특정위치 특정각도로 배치
    /// </summary>
    /// <param name="index">목적지 웨이포인트</param>
    /// <param name="position">소환 위치</param>
    /// <param name="angle">소환 각도</param>
    /// <returns>소환된 게임 오브젝트들</returns>
    public Enemy GetEnemy(int index, Vector3 position, float angle = 0.0f)
    {
        return enemyPool.GetObject(index, position, angle * Vector3.forward);
    }

    /// <summary>
    /// 데미지 텍스트 하나를 소환하는 함수
    /// </summary>
    /// <param name="damage">텍스트에 띄울 데미지</param>
    /// <returns></returns>
    public GameObject GetDamageText(int damage, Vector3? position)
    {
        return damageTextPool.GetObject(damage, position);
    }

    /// <summary>
    /// 아이템을 하나 생성하는함수
    /// </summary>
    /// <param name="code">생성할 아이템의 코드</param>
    /// <returns>아이템의 게임 오브젝트</returns>
    public GameObject MakeItem(ItemCode code)
    {
        ItemData data = GameManager.Instance.ItemData[code];    // 아이템 데이터 받아오기
        ItemObject obj = itemPool.GetObject();
        obj.ItemData = data;    // 풀에서 하나 꺼내고 데이터 설정
        return obj.gameObject;
    }

    /// <summary>
    /// 아이템을 여러개 생성하는함수
    /// </summary>
    /// <param name="code">생성할 아이템의 코드</param>
    /// <param name="positon">생성할 위치</param>
    /// <param name="useNoise">위치에 노이즈를 적용할지 여부(true면 적용,false면 안함)</param>
    /// <returns>아이템의 게임 오브젝트</returns>
    public GameObject MakeItem(ItemCode code, Vector3 positon, bool useNoise = false)
    {
        GameObject obj = MakeItem(code);
        Vector3 noise = Vector3.zero;
        if (useNoise)
        {
            Vector2 rand = Random.insideUnitCircle * noisePower;
            noise.x = rand.x;
            noise.z = rand.y;
        }
        obj.transform.position = positon + noise;
        return obj;
    }

    /// <summary>
    /// 아이템을 여러개 생성해는 함수
    /// </summary>
    /// <param name="code">생성할 아이템의 코드</param>
    /// <param name="count">생성할 개수</param>
    /// <returns>아이템의 게임 오브젝트들</returns>
    public GameObject[] MakeItems(ItemCode code, uint count)
    {

        GameObject[] items = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            items[i] = MakeItem(code);
        }

        return items;
    }

    /// <summary>
    /// 아이템을 여러개 생성해는 함수
    /// </summary>
    /// <param name="code">생성할 아이템의 코드</param>
    /// <param name="count">생성할 개수</param>
    /// <param name="positon">생성할 위치</param>
    /// <param name="useNoise">위치에 노이즈를 적용할지 여부(true면 적용,false면 안함)</param>
    /// <returns>아이템의 게임 오브젝트들</returns>
    public GameObject[] MakeItems(ItemCode code, uint count, Vector3 positon, bool useNoise = false)
    {
        GameObject[] items = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            items[i] = MakeItem(code, positon, useNoise);
        }

        return items;
    }

    /// <summary>
    /// 히트 이펙트를 생성하는 함수
    /// </summary>
    /// <param name="position">생성될 위치</param>
    /// <returns>소환된 오브젝트</returns>
    public GameObject GetHitEffect(Vector3? position)
    {
        return hitEffectPool.GetObject(position).gameObject;
    }
}

