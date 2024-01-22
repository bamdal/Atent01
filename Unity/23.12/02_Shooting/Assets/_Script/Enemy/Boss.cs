using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyBase
{
    [Header("보스 데이터")]
    /// <summary>
    /// 보스의 총알
    /// </summary>
    public PoolObjectType bullet = PoolObjectType.BossBullet;

    /// <summary>
    /// 보스의 미사일
    /// </summary>
    public PoolObjectType misslie = PoolObjectType.BossMissile;

    /// <summary>
    /// 총알 발사 간격
    /// </summary>
    public float bulletInterval = 1.0f;

    /// <summary>
    /// 보스 활동영역(최소, 월드좌표)
    /// </summary>
    public Vector2 areaMin = new Vector2(2, -3);

    /// <summary>
    /// 보스 활동영역(최대, 월드좌표)
    /// </summary>
    public Vector2 areaMax = new Vector2(7, 3);

    /// <summary>
    /// 미사일 일제사격 회수
    /// </summary>
    public int barrageCount = 3;

    /// <summary>
    /// 총알 발사 위치1
    /// </summary>
    Transform fire1;

    /// <summary>
    /// 총알 발사 위치2
    /// </summary>
    Transform fire2;

    /// <summary>
    /// 총알 발사 위치3
    /// </summary>
    Transform fire3;

    /// <summary>
    /// 보스의 이동방향
    /// </summary>
    Vector3 moveDirection = Vector3.left;    

    private void Awake()
    {
        Transform fireTransforms = transform.GetChild(1);
        fire1 = fireTransforms.GetChild(0);
        fire2 = fireTransforms.GetChild(1);
        fire3 = fireTransforms.GetChild(2);

    }

    protected override void OnMoveUpdate(float deltaTime)
    {
        transform.Translate(deltaTime * moveSpeed * moveDirection);
    }


    /// <summary>
    /// 보스 움직임 패턴용 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator MovePaternProcess()
    {
        moveDirection = Vector3.left; // 처음은 왼쪽으로 움직인다.
         
        float middleX = (areaMax.x-areaMin.x) *0.5f + areaMin.x; // area 가운데 구하기

        while (transform.position.x > middleX) // 중간지점에 도달할 때까지 계속 왼쪽으로 진행
        {
            yield return null;  
        }

        // 중간 지점에 도착
        ChangeDirection(); // 위아래로 움직이기
        StartCoroutine(FireBullet()); // 총알 발사

        while(true)
        {
            // 영역 최대, 영역 최소 지점 도착시 방향 전환
            if (transform.position.y >areaMax.y || transform.position.y <areaMin.y)
            {
                ChangeDirection();
                StartCoroutine(FireMisslie());

            }
            yield return null;
        }

    }

    void ChangeDirection()
    {
        Vector3 target = new Vector3();
        target.x = Random.Range(areaMin.x, areaMax.x);          // x 위치는 최소 ~ 최대 사이
        target.y = moveDirection.y > 0 ? areaMin.y : areaMax.y; // y 위치는 올라가던 중이면 최소, 내려가던중이면 최대
        if (transform.position.y > areaMax.y)
        {
            target.y = areaMin.y;
        }     
        if (transform.position.y < areaMin.y)
        {
            target.y = areaMax.y;
        }

        moveDirection = (target-transform.position).normalized;// 방향 수정

    }

    /// <summary>
    /// 보스의 스폰이 완료된 후에 마지막으로 실행되는 함수
    /// </summary>
    public void OnSpawn()
    {
        StopAllCoroutines(); // 꺼낼 때 실행되었던 모든 코루틴 종료

        StartCoroutine(MovePaternProcess());

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 p0 = new(areaMin.x, areaMin.y);
        Vector3 p1 = new(areaMax.x, areaMin.y);
        Vector3 p2 = new(areaMax.x, areaMax.y);
        Vector3 p3 = new(areaMin.x, areaMax.y);
        Gizmos.DrawLine(p0,p1);
        Gizmos.DrawLine(p1,p2);
        Gizmos.DrawLine(p2,p3);
        Gizmos.DrawLine(p3,p0);

  
    }

    /// <summary>
    /// 총알 발사 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator FireBullet()
    {
        while (true)
        {
            Factory.Instance.GetObject(PoolObjectType.BossBullet, fire1.position);
            Factory.Instance.GetObject(PoolObjectType.BossBullet, fire2.position);

            yield return new WaitForSeconds(bulletInterval);
        }

    }

    /// <summary>
    /// 미사일 빠르게 연사하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator FireMisslie()
    { 
        for(int i = 0; i < barrageCount; i++) 
        {
            Factory.Instance.GetObject(PoolObjectType.BossMissile,fire3.position);
            yield return new WaitForSeconds(0.2f);
        }
    }
    /*
        protected override void OnInitialize()
        {
            base.OnInitialize();
            StartCoroutine(AppearProcess());
        }

        IEnumerator AppearProcess()
        {
            Vector3 up = transform.up;
            while (true)
            {
                transform.Translate(Time.deltaTime * moveSpeed * -transform.right, Space.World); // 기본 동작(왼쪽으로 이동)

                yield return new WaitForSeconds(0.001f);
                if (transform.position.x < (areaMax.x - areaMin.x))
                {

                    break;
                }
            }
            StartCoroutine(ShootBullet());
            while (true)
            {

                transform.Translate(Time.deltaTime * moveSpeed * up, Space.World); 
                yield return new WaitForSeconds(0.001f);

                if (transform.position.y < areaMin.y)
                {
                    up *= -1;
                    StartCoroutine(ShootMissile());

                }

                if (transform.position.y > areaMax.y)
                {
                    up *= -1;
                    StartCoroutine(ShootMissile());


                }


            }

        }
        IEnumerator ShootBullet()
        {
            while (true)
            {
                Factory.Instance.GetBossBullet(fire1.position);
                Factory.Instance.GetBossBullet(fire2.position);
                yield return new WaitForSeconds(0.5f);

            }
        }

        IEnumerator ShootMissile()
        {
            for(int i = 0; i < barrageCount; i++) 
            {
                Factory.Instance.GetBossMissile(fire3.position);
                yield return new WaitForSeconds(0.1f);
            }

        }


        protected override void OnDie()
        {
            StopAllCoroutines();
            base.OnDie();

        }*/
}

// 스폰되면 정해진 영역의 가운데까지 전진
// 특정 영역 안에서 위아래로 왕복한다.
// 계속 주기적으로 총알을 발사한다.
// 이동 방향을 변경할 때 미사일을 3발 연속 발사한다.
