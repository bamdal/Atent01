using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : EnemyBase
{
    [Header("파워업 아이템을 주는 적 데이터")]

    // 등장하고 약간의 시간이 지난 후 일정시간 동안 대기한다
    // 죽을 때 파워업 아이템을 떨군다.

    /// <summary>
    /// 등장 시간(처음 멈출때까지의 시간)
    /// </summary>
    public float appearTime = 0.5f;
    /// <summary>
    /// 대기 시간
    /// </summary>
    public float waitTime = 5.0f;

    /// <summary>
    /// 대기 시간 이후의 속도
    /// </summary>
    public float secondSpeed = 10.0f;

    /// <summary>
    /// 드랍할 아이템 종류
    /// </summary>
    public PoolObjectType bonusType = PoolObjectType.PowerUp;

    Animator animator;

    readonly int SpeedHash = Animator.StringToHash("Speed");
    //readonly int IsMoving = Animator.StringToHash("IsMoving");
    protected override void OnInitialize()
    {
        base.OnInitialize();

        StopAllCoroutines();
        animator = GetComponent<Animator>();
        StartCoroutine(AppearProcess());
    }

    IEnumerator AppearProcess()
    {
        animator.SetFloat(SpeedHash, moveSpeed);
        yield return new WaitForSeconds(appearTime);
        moveSpeed = 0.0f;
        animator.SetFloat(SpeedHash, moveSpeed);
        yield return new WaitForSeconds(waitTime);
        moveSpeed = secondSpeed;
        animator.SetFloat(SpeedHash, moveSpeed);
    }

    protected override void OnMoveUpdate(float deltaTime)
    {
        base.OnMoveUpdate(deltaTime);
        /*        if (moveSpeed == 0.0f)
                {
                    animator.SetBool(IsMoving, false);
                }
                else
                {
                    animator.SetBool(IsMoving, true);

                }
        */
    }

    protected override void OnDie()
    {
        Factory.Instance.GetObject(bonusType, transform.position);
        base.OnDie();
    }
}

// 실습
// 1. 움직일 때 뒤 스러스트 부분의 alpha가 깜빡인다.
// 2. 대기할 때는 스러스트가 안보인다.

// 3. 커브 적 만들기 Curve
// 4. 생성 위치의 y가 0보다 크면 좌회전 0보다 작으면 우회전