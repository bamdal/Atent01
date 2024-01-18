using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86.Avx;

/// <summary>
/// 오브젝트 풀을 사용하는 오브젝트의 종류
/// </summary>
public enum PoolObjectType
{
    PlayerBullet = 0,   // 플레이어 총알
    Hit,                // 총알이 터지는 이펙트
    ExplosionEffect,    // 적이 터지는 이펙트
    PowerUp,            // 파워업 아이템
    EnemyWave,          // 적
    EnemyAsteroid,      // 큰 유성 
    EnemyAsteroidMini,  // 작은 유성
    EnemyBonus,         // 적(파워업 아이템 드랍)
    EnemyCurve,          // 적 커브
    BossBullet

}

public class Factory : Singleton<Factory>
{
    // 오브젝트 풀들
    BulletPool bullet;
    HitPool hit;
    ExplosionPool explosion;
    PowerUpPool powerUp;
    WavePool enemy;
    AsteroidPool asteroid;
    AsteroidMiniPool asteroidMini;
    BonusPool bonus;
    CurvePool curve;
    BossBulletPool bossBullet;


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
        powerUp = GetComponentInChildren<PowerUpPool>();
        if(powerUp != null)
            powerUp.Initialize();
        enemy = GetComponentInChildren<WavePool>();
        if (enemy != null)
            enemy.Initialize();
        asteroid = GetComponentInChildren<AsteroidPool>();
        if (asteroid != null)
            asteroid.Initialize();
        asteroidMini = GetComponentInChildren<AsteroidMiniPool>();
        if (asteroidMini != null)
            asteroidMini.Initialize();
        bonus = GetComponentInChildren<BonusPool>();
        if (bonus != null)
            bonus.Initialize();
        curve = GetComponentInChildren<CurvePool>();
        if (curve != null)
            curve.Initialize();
        bossBullet = GetComponentInChildren<BossBulletPool>();
        if (bossBullet != null)
            bossBullet.Initialize();
            
            
    }

    /// <summary>
    /// 풀에 있는 게임오브젝트를 하나 가져오기
    /// </summary>
    /// <param name="type">가져올 오브젝트의 종류</param>
    /// <param name="position">오브젝트가 배치될 위치</param>
    /// <param name="angle">오브젝트 초기 각도</param>
    /// <returns>활성화된 오브젝트</returns>
    public GameObject GetObject(PoolObjectType type,Vector3? position = null, Vector3? euler = null)
    {
        GameObject result = null;
        switch (type)
        {
            case PoolObjectType.PlayerBullet:
                result = bullet.GetObject(position, euler).gameObject;
                break;
            case PoolObjectType.Hit:
                result = hit.GetObject(position, euler).gameObject;
                break;
            case PoolObjectType.ExplosionEffect:
                result = explosion.GetObject(position, euler).gameObject;
                break;
            case PoolObjectType.PowerUp:
                result = powerUp.GetObject(position, euler).gameObject; 
                break;
            case PoolObjectType.EnemyWave:
                result = enemy.GetObject(position, euler).gameObject;
                break;
            case PoolObjectType.EnemyAsteroid:
                result = asteroid.GetObject(position, euler).gameObject;
                break;
            case PoolObjectType.EnemyAsteroidMini:
                result = asteroidMini.GetObject(position, euler).gameObject;
                break;
            case PoolObjectType.EnemyBonus:
                result = bonus.GetObject(position, euler).gameObject;
                break;
            case PoolObjectType.EnemyCurve:
                result = curve.GetObject(position, euler).gameObject;
                break;
            case PoolObjectType.BossBullet:
                result = bossBullet.GetObject(position, euler).gameObject;
                break;
            default:
                break;
        }

        return result;
    }
/*
    /// <summary>
    /// 풀에 있는 게임 오브젝트 하나 가져와서 특정 위치에 배치
    /// </summary>
    /// <param name="type">가져올 오브젝트의 종류</param>
    /// <param name="position">오브젝트가 배치될 위치</param>
    /// <param name="angle">오브젝트 초기 각도</param>
    /// <returns>활성화된 오브젝트</returns>
    public GameObject GetObject(PoolObjectType type, Vector3 position , float angle = 0.0f)
    {
        GameObject obj = GetObject(type); // 가져와서
        obj.transform.position = position; // 위치적용
        obj.transform.Rotate(angle * Vector3.forward);

 *//*       switch (type) // 개별적으로 추가 처리가 필요한 오브젝트들
        {
            case PoolObjectType.EnemyWave:
                Wave enemy = obj.GetComponent<Wave>();
                enemy.SetStartPosition(position); // 적의 spawnY 지정
                break;
        }*//*
        return obj;
    }
*/
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
    public Bullet GetBullet(Vector3 position, float angle = 0.0f)
    {
/*        Bullet comp = bullet.GetObject();
        comp.transform.position = position;  
        comp.transform.Rotate(angle * Vector3.forward);*/
        return bullet.GetObject(position, angle * Vector3.forward); ;
    }

    public HitEffect GetHitEffect()
    {
        return hit.GetObject();
    }

    public HitEffect GetHitEffect(Vector3 position, float angle = 0.0f)
    {


        return hit.GetObject(position, angle * Vector3.forward); ;
    }

    public Explosion GetExplosionEffect()
    {
        return explosion.GetObject();
    }

    public Explosion GetExplosionEffect(Vector3 position, float angle = 0.0f)
    {

        return explosion.GetObject(position, angle * Vector3.forward);
    }

    public Wave GetEnemyWave()
    {
        return enemy.GetObject();
    }

    public Wave GetEnemyWave(Vector3 position, float angle = 0.0f)
    {
/*        Wave comp = enemy.GetObject();
        comp.SetStartPosition(position); // 적의 spanwY 지정하기 위한 용도
        comp.transform.Rotate(angle * Vector3.forward);
*/
        return enemy.GetObject(position,angle*Vector3.forward);
    }

    public Asteroid GetAsteroid()
    {
        return asteroid.GetObject();
    }

    public Asteroid GetAsteroid(Vector3 position, float angle = 0.0f)
    {
        return asteroid.GetObject(position, angle * Vector3.forward);
    }
    public AsteroidMini GetAsteroidMini()
    {
        return asteroidMini.GetObject();
    }

    public AsteroidMini GetAsteroidMini(Vector3 position, float angle = 0.0f)
    {
        return asteroidMini.GetObject(position,angle * Vector3.forward);
       
    }

    
    public PowerUp GetPowerUp()
    {
        return powerUp.GetObject();
    }

    public PowerUp GetPowerUp(Vector3 position, float angle = 0.0f)
    {
        return powerUp.GetObject(position,angle * Vector3.forward);
       
    }    
    public Bonus GetBonus()
    {
        return bonus.GetObject();
    }

    public Bonus GetBonus(Vector3 position, float angle = 0.0f)
    {
        return bonus.GetObject(position,angle * Vector3.forward);
       
    }
     
    public Curve GetCurve()
    {
        return curve.GetObject();
    }

    public Curve GetCurve(Vector3 position, float angle = 0.0f)
    {
        return curve.GetObject(position,angle * Vector3.forward);
       
    }   
    public BossBullet GetBossBullet()
    {
        return bossBullet.GetObject();
    }

    public BossBullet GetBossBullet(Vector3 position, float angle = 0.0f)
    {
        return bossBullet.GetObject(position,angle * Vector3.forward);
       
    }



}
