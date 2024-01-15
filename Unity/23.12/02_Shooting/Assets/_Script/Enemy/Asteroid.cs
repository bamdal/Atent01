
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Asteroid : EnemyBase
{

    /*    public float speed = 2.0f;
        float spawnY = 0.0f;
        Vector3 spawnPos = Vector3.zero;
        private void Update()
        {
            //transform.Rotate(0, 0, 1f);
            transform.Translate(Time.deltaTime * speed *spawnPos);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            spawnY = transform.position.y;

        }

        protected override void OnDisable()
        {
            base.OnDisable();


        }

        public void SetStartPosition(Vector3 position)
        {
            transform.position = position;
            spawnPos = position;
            spawnY = position.y;
        }*/


    /// <summary>
    /// 이동방향
    /// </summary>
    Vector3 direction = Vector3.zero;

    [Header("큰 운석 data")]
    // 이동속도 랜덤
    // 회전속도 랜덤
    // 큰 운석은 수명을 가진다.
    // 수명도 랜덤하다
    // 큰 운석은 죽을 때 작은 운석을 랜덤한 개수를 생성한다
    // 모든 작은 운석은 서로 같은 사이각을 가진다 ( 작은 운석이 6개 나오면 사이각은 60도)
    // criticalRate 확률로 작은 운석을 20개 생성

    public float minMoveSpeed = 2.0f;
    public float maxMoveSpeed = 4.0f;

    public float minRotateSpeed = 30.0f;
    public float maxRotateSpeed = 360.0f;

    public float minLifeTime = 4.0f;
    public float maxLifeTime = 7.0f;

    public int MinMiniCount = 3;
    public int MaxMiniCount = 7;

    [Range(0f, 1f)]
    public float criticalRate = 0.05f;
    public int criticalMiniCount = 20;
    /// <summary>
    /// 회전 속도
    /// </summary>
    public float rotationSpeed = 360.0f;


    /// <summary>
    /// 원래 점수(자폭했을 때 점수를 안주기 위해 필요)
    /// </summary>
    int originalScore;

    private void Awake()
    {
        originalScore = score;
    }


    protected override void OnInitialize()
    {
        base.OnInitialize();
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        rotationSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);
        score = originalScore;

        StartCoroutine(SelfCrush());
    }


    /// <summary>
    /// 목적지를 이용해 방향을 결정하는 함수
    /// </summary>
    /// <param name="destination"></param>
    public void SetDestination(Vector3 destination)
    {
        direction = (destination - transform.position).normalized;

    }




    protected override void OnMoveUpdate(float deltaTime)
    {
        transform.Translate(Time.deltaTime * moveSpeed * direction, Space.World); // 월드기준으로 이동
        transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
    }
    protected override void OnEnable()
    {

        base.OnEnable();
    }


    protected override void OnDisable()
    {
        
        base.OnDisable();
       
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + direction); // 진행방향 표시
    }
    // 실습
    // 1. 운석은 만들어졌을때 지정된 방향으로 움직인다.
    // 2. 운석은 계속 회전해야 한다.
    // 3. 운석은 오브젝트 풀에서 관리되어야 한다.




    // 스포너에서 생성할때 풀에서 만들기

    IEnumerator SelfCrush()
    {
        float lifeTime = Random.Range(minLifeTime, maxLifeTime);
        
        yield return new WaitForSeconds(lifeTime);
        score = 0;
        OnDie();
    }

    protected override void OnDie()
    {
        int count = criticalMiniCount; ;
        if(Random.value > criticalRate) 
        {
            count = Random.Range(MinMiniCount,MaxMiniCount);
        }

        float angle = 360.0f / count;
        float startAngle = Random.Range(0.0f, 360.0f);

        for(int i = 0; i < count; i++) 
        {
            Factory.Instance.GetAsteroidMini(transform.position, startAngle + angle * i);
        }

        base.OnDie();
        
    }
}
