using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 실습
    // 1. 적을 스폰한다.
    // 2. 랜덤한 높이에서 생성된다 (y : +4 ~ -4)
    /*    float rand;
        public GameObject EnemyPrefeb;
        float time=0;*/

    
    public float interval = 0.5f;

    protected const float MinY = -4.0f;
    protected const float MaxY = 4.0f;

    //float elapsedTime = 0.0f;
    //int spwanCounter=0;
 

    private void Awake()
    {

    }

    private void Start()
    {
        //elapsedTime = 0.0f;
        //spwanCounter = 0;
        StartCoroutine(SpawnCoroution());
    }

    IEnumerator SpawnCoroution()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);  // interval만큼기다린수
            Spawn();                                    // Spawn()실행
        }

    }

    private void Update()
    {

/*        time += Time.deltaTime;
        if (time>1)
        {
            rand = Random.Range(-4.0f, 4.0f);
            time = 0;
            Instantiate(EnemyPrefeb,new Vector3(transform.position.x, rand,0),Quaternion.identity);
        }*/
        
/*        elapsedTime += Time.deltaTime;
        if (elapsedTime > interval)
        {
            elapsedTime = 0.0f;
            Spawn();
        }*/

        
    }

    protected virtual void Spawn() // 적을 하나 스폰
    {
        //GameObject obj = Instantiate(enemyPrefeb, GetSpawnPosition(),Quaternion.identity);
        //obj.transform.SetParent(transform);
        //obj.name = $"Enemy_{spwanCounter}";
        //spwanCounter++;

        Factory.Instance.GetEnemyWave(GetSpawnPosition());
        //Enemy enemy = Factory.Instance.GetEnemy(GetSpawnPosition());
        //enemy.transform.SetParent(transform);
    }

    protected Vector3 GetSpawnPosition() // 스폰할 위치를 리턴
    {
        Vector3 pos = transform.position;
        pos.y += UnityEngine.Random.Range(MinY, MaxY);

        return pos;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.green;                             //색상 지정
        Vector3 p0 = transform.position + Vector3.up * MaxY;    //선의 시작점
        Vector3 p1 = transform.position + Vector3.up * MinY;    //선의 도착점
        Gizmos.DrawLine(p0, p1);                                //시작점에서 도착점까지 그려줌
        

    }

    protected virtual void OnDrawGizmosSelected()
    {
        // 이 오브젝트를 선택했을 때 사각형 그리기
        /*        Gizmos.color = Color.blue;
                Vector3[] points;
                int p = 15;
                points = new Vector3[8]
                {
                    new Vector3(-p, 0, 0),
                    new Vector3(p, 0, 0),
                    new Vector3(-p, p, 0),
                    new Vector3(p, p, 0),
                    new Vector3(-p, 0, 0),
                    new Vector3(-p, p, 0),
                    new Vector3(p, p, 0),
                    new Vector3(p, 0, 0)
                };

                Gizmos.DrawLineList(points);

        */

        Gizmos.color = Color.red;                             // 색깔 지정        
        Vector3 p0 = transform.position + Vector3.up * MaxY - Vector3.right * 0.5f;
        Vector3 p1 = transform.position + Vector3.up * MaxY + Vector3.right * 0.5f;
        Vector3 p2 = transform.position + Vector3.up * MinY + Vector3.right * 0.5f;
        Vector3 p3 = transform.position + Vector3.up * MinY - Vector3.right * 0.5f;
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p0);

    }
}
