using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MultiSpawner;

public class MultiSpawner : MonoBehaviour
{
    // 인스펙터창에서 맴버변수인 구조체나 클래스 내부를 확인하고 싶으면 Serializable속성 연결
    [Serializable]
    public struct SpawnData
    {
        public SpawnData(PoolObjectType type = PoolObjectType.EnemyWave, float interval = 0.5f)
        {
            this.spawnType = type;
            this.interval = interval;
        }
        public PoolObjectType spawnType;
        public float interval;

    }
    public SpawnData[] spawnDatas;


    protected const float MinY = -4.0f;
    protected const float MaxY = 4.0f;

    Transform asteroidDestination;


    private void Awake()
    {
        asteroidDestination = transform.GetChild(0);
    }

    private void Start()
    {
        foreach (var data in spawnDatas)
        {
            StartCoroutine(SpawnCoroution(data));
        }
    }

    IEnumerator SpawnCoroution(SpawnData data)
    {
        while (true)
        {
            yield return new WaitForSeconds(data.interval);  // interval만큼기다린수
            float height = UnityEngine.Random.Range(MinY, MaxY);
            GameObject obj = Factory.Instance.GetObject(data.spawnType, new(transform.position.x, height, 0.0f));

            switch (data.spawnType)
            {
                case PoolObjectType.EnemyAsteroid:
                    Asteroid asteroid = obj.GetComponent<Asteroid>();
                    asteroid.SetDestination(GetDestination());
                    break;
            }

        }

    }
    /// <summary>
    /// 운석의 목적지를 랜덤으로 구해주는 함수
    /// </summary>
    /// <returns>운석의 목적지</returns>
    Vector3 GetDestination()
    {
        Vector3 pos = asteroidDestination.position;
        pos.y += UnityEngine.Random.Range(MinY, MaxY);

        return pos;
    }



    private void Spawn(SpawnData data) // 적을 하나 스폰
    {



    }

    protected Vector3 GetSpawnPosition() // 스폰할 위치를 리턴
    {
        Vector3 pos = transform.position;
        pos.y += UnityEngine.Random.Range(MinY, MaxY);

        return pos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;                             //색상 지정
        Vector3 p0 = transform.position + Vector3.up * MaxY;    //선의 시작점
        Vector3 p1 = transform.position + Vector3.up * MinY;    //선의 도착점
        Gizmos.DrawLine(p0, p1);                                //시작점에서 도착점까지 그려줌

        if (asteroidDestination == null)
        {
            asteroidDestination = transform.GetChild(0);
        }

        Gizmos.color = Color.green;                             //색상 지정
        Vector3 p2 = asteroidDestination.position + Vector3.up * MaxY;    //선의 시작점
        Vector3 p3 = asteroidDestination.position + Vector3.up * MinY;    //선의 도착점
        Gizmos.DrawLine(p2, p3);                                //시작점에서 도착점까지 그려줌

    }

    private void OnDrawGizmosSelected()
    {


        Gizmos.color = Color.red;                             // 색깔 지정        
        Vector3 p0 = transform.position + Vector3.up * MaxY - Vector3.right * 0.5f;
        Vector3 p1 = transform.position + Vector3.up * MaxY + Vector3.right * 0.5f;
        Vector3 p2 = transform.position + Vector3.up * MinY + Vector3.right * 0.5f;
        Vector3 p3 = transform.position + Vector3.up * MinY - Vector3.right * 0.5f;
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p0);


        Gizmos.color = Color.red;                             // 색깔 지정        
        Vector3 p4 = asteroidDestination.position + Vector3.up * MaxY - Vector3.right * 0.5f;
        Vector3 p5 = asteroidDestination.position + Vector3.up * MaxY + Vector3.right * 0.5f;
        Vector3 p6 = asteroidDestination.position + Vector3.up * MinY + Vector3.right * 0.5f;
        Vector3 p7 = asteroidDestination.position + Vector3.up * MinY - Vector3.right * 0.5f;
        Gizmos.DrawLine(p4, p5);
        Gizmos.DrawLine(p5, p6);
        Gizmos.DrawLine(p6, p7);
        Gizmos.DrawLine(p7, p4);
    }
}
