using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 오브젝트 풀을 사용하는 오브젝트의 종류
/// </summary>
public enum PoolObjectType
{
    PlayerBullet = 0,   // 플레이어 총알
    Hit,                // 총알이 터지는 이펙트
    ExplosionEffect,    // 적이 터지는 이펙트
    EnemyWave,              // 적
    EnemyAsteroid            // 유성 
}

public class Factory : Singleton<Factory>
{
    // 오브젝트 풀들
    BulletPool bullet;
    HitPool hit;
    ExplosionPool explosion;
    EnemyPool enemy;
    AsteroidPool asteroid;

    /// <summary>
    /// 씬이 로딩이 완료 될때마다 실행되는 초기화 함수
    /// </summary>
    protected override void OnInitialize()
    {
        base.OnInitialize();

        // 풀 컴모넌트를 찾고 찾으면 초기화
        bullet = GetComponentInChildren<BulletPool>(true); // 나와 내 자식 오브젝트에서 컴포넌트 찾음 (true)면 비활성화 된것도 찾음
        if (bullet != null)
            bullet.Initialize();
        hit = GetComponentInChildren<HitPool>();
        if (hit != null)
            hit.Initialize();
        explosion = GetComponentInChildren<ExplosionPool>();
        if (explosion != null)
            explosion.Initialize();
        enemy = GetComponentInChildren<EnemyPool>();
        if (enemy != null)
            enemy.Initialize();
        asteroid = GetComponentInChildren<AsteroidPool>();
        if (asteroid != null)
            asteroid.Initialize();
            
            
    }

    /// <summary>
    /// 풀에 있는 게임오브젝트를 하나 가져오기
    /// </summary>
    /// <param name="type">가져올 오브젝트의 종류</param>
    /// <returns>활성화된 오브젝트</returns>
    GameObject GetObject(PoolObjectType type)
    {
        GameObject result = null;
        switch (type)
        {
            case PoolObjectType.PlayerBullet:
                result = bullet.GetObject().gameObject;
                break;
            case PoolObjectType.Hit:
                result = hit.GetObject().gameObject;
                break;
            case PoolObjectType.ExplosionEffect:
                result = explosion.GetObject().gameObject;
                break;
            case PoolObjectType.EnemyWave:
                result = enemy.GetObject().gameObject;
                break;
            case PoolObjectType.EnemyAsteroid:
                result = asteroid.GetObject().gameObject;
                break;
            default:
                break;
        }

        return result;
    }

    /// <summary>
    /// 풀에 있는 게임 오브젝트 하나 가져와서 특정 위치에 배치
    /// </summary>
    /// <param name="type">가져올 오브젝트의 종류</param>
    /// <param name="position">오브젝트가 배치될 위치</param>
    /// <returns>활성화된 오브젝트</returns>
    public GameObject GetObject(PoolObjectType type, Vector3 position)
    {
        GameObject obj = GetObject(type); // 가져와서
        obj.transform.position = position; // 위치적용


        switch (type) // 개별적으로 추가 처리가 필요한 오브젝트들
        {
            case PoolObjectType.EnemyWave:
                Wave enemy = obj.GetComponent<Wave>();
                enemy.SetStartPosition(position); // 적의 spawnY 지정
                break;
        }
        return obj;
    }

    /// <summary>
    /// 총알 하나 가져오는 함수
    /// </summary>
    /// <returns>활성화된 총알</returns>
    public Bullet GetBullet()
    {
        return bullet.GetObject();
    }

    /// <summary>
    /// 총알 하나 가져와서 특정위치에 배치하는 함수
    /// </summary>
    /// <param name="position">배치될 위치</param>
    /// <returns>활성화된 총알</returns>
    public Bullet GetBullet(Vector3 position)
    {
        Bullet bulletComp = bullet.GetObject();
        bulletComp.transform.position = position;   
        return bulletComp;
    }

    public HitEffect GetHitEffect()
    {
        return hit.GetObject();
    }

    public HitEffect GetHitEffect(Vector3 position)
    {
        HitEffect hitComp = hit.GetObject();
        hitComp.transform.position = position;
        return hitComp;
    }

    public Explosion GetExplosionEffect()
    {
        return explosion.GetObject();
    }

    public Explosion GetExplosionEffect(Vector3 position)
    {
        Explosion comp = explosion.GetObject();
        comp.transform.position = position; 
        return comp;
    }

    public Wave GetEnemyWave()
    {
        return enemy.GetObject();
    }

    public Wave GetEnemyWave(Vector3 position)
    {
        Wave enemyComp = enemy.GetObject();
        enemyComp.SetStartPosition(position); // 적의 spanwY 지정하기 위한 용도
        return enemyComp;
    }

    public Asteroid GetAsteroid()
    {
        return asteroid.GetObject();
    }

    public Asteroid GetAsteroid(Vector3 position)
    {
        Asteroid comp = asteroid.GetObject();
        comp.transform.position=position;
        return comp;
    }


}
