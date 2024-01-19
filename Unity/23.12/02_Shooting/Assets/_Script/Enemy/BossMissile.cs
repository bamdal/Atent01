using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMissile : EnemyBase
{

    [Header("추적 미사일 데이터")]
    /// <summary>
    /// 미사일의 유도 성능
    /// </summary>
    public float guiedPerformance = 1.5f;


    /// <summary>
    /// 추적 대상(플레이어)
    /// </summary>
    Transform target;
    
    /// <summary>
    /// 유도중인지 표시(true면 유도, false면 유도 중지
    /// </summary>
    bool OnGuied = true;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        target = GameManager.Instance.Player.transform; // 활성화 때마다 플레이어 찾기
        OnGuied = true;                                 // 유도 켜기
    }

    protected override void OnMoveUpdate(float deltaTime)
    {
        base.OnMoveUpdate(deltaTime);
        if(OnGuied) // 유도 중이면
        {
            

            Vector3 dir = target.position - transform.position; // 타겟으로 가는 방향 구하고
            //transform.right = -dir; // 방향 돌리기
            transform.right = -Vector3.Slerp(-transform.right,dir,deltaTime* guiedPerformance);
            // 시작방향에서 목표로 하는 방향으로 대략 2초에 걸쳐서 변경되는 속도
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(OnGuied && collision.CompareTag("Player")) // 유도 중일 때 플레이어가 트리거 영역에 들어오면
        {
            OnGuied = false;    //유도 중지
        }
    }
}
