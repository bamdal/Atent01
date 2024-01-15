using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : EnemySpawner
{
    // 스포너
    // 1. 운석 생성용 스포너가 있어야 한다
    // 2. 운석을 생성하고 시작점과 도착점을 지정한다.
    // 3. 도착점의 범위가 씬창에서 보여야 한다

    /*    public GameObject enemyPrefeb;
        public float interval = 0.5f;

        const float startMinY = -4.0f;
        const float startMaxY = 4.0f;
        const float endMinY = -4.0f;
        const float endMaxY = 4.0f;

        Vector3 startPosition = Vector3.zero;
        Vector3 endPosition = Vector3.zero;

        private void Start()
        {
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

        private void Spawn() // 적을 하나 스폰
        {
            startPosition = new Vector3(10.0f,Random.Range(startMinY, startMaxY));
            endPosition = new Vector3(-10.0f,Random.Range(endMinY, endMaxY));
            Asteroid asteroid = Factory.Instance.GetAsteroid(new Vector3(startPosition.x,endPosition.y));

        }


        private void OnDrawGizmos()
        {

        }*/

    Transform destinationArea;

    private void Awake()
    {
        destinationArea = transform.GetChild(0);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if(destinationArea == null )
        {
            destinationArea = transform.GetChild(0);
        }

        Gizmos.color = Color.green;                             //색상 지정
        Vector3 p0 = destinationArea.position + Vector3.up * MaxY;    //선의 시작점
        Vector3 p1 = destinationArea.position + Vector3.up * MinY;    //선의 도착점
        Gizmos.DrawLine(p0, p1);                                //시작점에서 도착점까지 그려줌

    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (destinationArea == null)
        {
            destinationArea = transform.GetChild(0);
        }
        Gizmos.color = Color.red;                             // 색깔 지정        
        Vector3 p0 = destinationArea.position + Vector3.up * MaxY - Vector3.right * 0.5f;
        Vector3 p1 = destinationArea.position + Vector3.up * MaxY + Vector3.right * 0.5f;
        Vector3 p2 = destinationArea.position + Vector3.up * MinY + Vector3.right * 0.5f;
        Vector3 p3 = destinationArea.position + Vector3.up * MinY - Vector3.right * 0.5f;
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p0);
    }
    protected override void Spawn()
    {
        Asteroid asteroid = Factory.Instance.GetAsteroid(GetSpawnPosition());
        asteroid.SetDestination(GetDestination());
    }

    Vector3 GetDestination()
    {
        Vector3 pos = destinationArea.position;
        pos.y += UnityEngine.Random.Range(MinY, MaxY);
        
        return pos;
    }
}
