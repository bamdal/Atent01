using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Enemy : RecycleObject, IBattler, IHealth
{
    /// <summary>
    /// 적이 가질수 있는 상태의종류
    /// </summary>
    protected enum EnemyState
    {
        Wait = 0,   // 대기
        Patrol,     // 순찰
        Chase,      // 추적
        Attack,     // 공격
        Dead        // 사망
    }

    /// <summary>
    /// 적의 현재 상태
    /// </summary>
    EnemyState state = EnemyState.Patrol;

    /// <summary>
    /// 상태를 설정하고 확인하는 프로퍼티
    /// </summary>
    protected EnemyState State
    {
        get => state;
        set
        {
            if (state != value)
            {
                state = value;
                switch (state)  // 상태에 진입할 때 할 일들 처리
                {
                    case EnemyState.Wait:
                        // 일정 시간 대기
                        //Debug.Log("대기");
                        agent.isStopped = true;                 // agent 정지 
                        agent.velocity = Vector3.zero;          // agnet에 남아있던 운동량 제거
                        animator.SetTrigger(anim_StopToHash);   // 애니메이션 정지
                        WaitTimer = waitTime;                   // 기다려야 하는 시간 초기화
                        onStateUpdate = Update_Wait;            // 대기 상태용 업데이트 함수
                        break;
                    case EnemyState.Patrol:
                        //Debug.Log("패트롤");
                        agent.isStopped = false;                        // agnet다시 켜기
                        agent.SetDestination(waypoints.NextTarget);     // 목적지 지정(웨이포인트 지점)
                        animator.SetTrigger(anim_MoveToHash);           // 애니메이션 재생
                        onStateUpdate = Update_Patrol;                  // 추적용 업데이트 함수
                        break;
                    case EnemyState.Chase:
                        agent.isStopped = false;                        // agnet다시 켜기
            
                        animator.SetTrigger(anim_MoveToHash);           // 애니메이션 재생

                        onStateUpdate = Update_Chase;
                        break;
                    case EnemyState.Attack:
                        onStateUpdate = Update_Attack;
                        break;
                    case EnemyState.Dead:
                        agent.isStopped = true;
                        agent.velocity = Vector3.zero;
                        animator.SetTrigger(anim_DieToHash);
                        onStateUpdate = Update_Dead;
                        break;
                }

            }
        }
    }

    /// <summary>
    /// 대기 상태로 들어갔을 때 기다리는 시간
    /// </summary>
    public float waitTime = 1.0f;

    /// <summary>
    /// 대기 시간 측정용(계속 감소)
    /// </summary>
    float waitTimer = 1.0f;

    /// <summary>
    /// 측정용 시간 처리용 프로퍼티
    /// </summary>
    protected float WaitTimer
    {
        get => waitTimer;
        set
        {
            waitTimer = value;

            if (waitTimer < 0.0f)
            {
                State = EnemyState.Patrol;
            }
        }
    }

    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed = 3.0f;

    /// <summary>
    /// 적이 순찰할 웨이포인트(public 이지만 private처럼 사용)
    /// </summary>
    public Waypoints waypoints;

    /// <summary>
    /// 웨이포인트 확인 및 설정용 프로퍼티
    /// </summary>



    /// <summary>
    /// 원거리 시야범위
    /// </summary>
    public float farSightRange = 10.0f;

    /// <summary>
    /// 원거리 시야각의 절반
    /// </summary>
    public float sightHalfAngle = 50.0f;

    /// <summary>
    /// 근거리 시야 범위
    /// </summary>
    public float nearSightRange = 1.5f;

    /// <summary>
    /// 추적 대상의 트랜스폼
    /// </summary>
    protected Transform chaseTarget = null;

    /// <summary>
    /// 공격 대상
    /// </summary>
    protected IBattler attackTarget = null;

    /// <summary>
    /// 공격력(변수는 인스펙터에서 수정하기 위해 public으로 만든 것임)
    /// </summary>
    public float attackPower = 10.0f;

    /// <summary>
    /// 공격력
    /// </summary>
    public float AttackPower => attackPower;

    /// <summary>
    /// 방어력(변수는 인스펙터에서 수정하기 위해 public으로 만든 것임)
    /// </summary>
    public float defencePower = 3.0f;

    /// <summary>
    /// 방어력
    /// </summary>
    public float DefencePower => defencePower;

    /// <summary>
    /// 공격 속도
    /// </summary>
    public float attaclSpeed = 1.0f;

    /// <summary>
    /// 남아있는 쿨타임
    /// </summary>
    float attackCoolTime = 0.0f;


    /// <summary>
    /// HP
    /// </summary>
    protected float hp = 100.0f;

    public float HP
    {
        get => hp;
        set
        {
            hp = value;
            if (State != EnemyState.Dead && hp <= 0.0f)  // 한번만 죽기용도
            {
                Die();

            }
            hp = Mathf.Clamp(hp, 0.0f, MaxHP);
            onHealthChange?.Invoke(hp / MaxHP);
        }
    }

    /// <summary>
    /// 최대 HP(변수는 인스펙터에서 수정하기 위해 public으로 만든 것임)
    /// </summary>
    public float maxHP = 100.0f;

    public float MaxHP => maxHP;

    /// <summary>
    /// HP 변경시 실행되는 델리게이트
    /// </summary>
    public Action<float> onHealthChange { get; set; }


    /// <summary>
    /// 살았는지 죽었는지 확인하기 위한 프로퍼티
    /// </summary>
    public bool IsAlive => HP > 0.0f;

    /// <summary>
    /// 이 캐릭터가 죽었을 때 실행되는 델리게이트
    /// </summary>
    public Action onDie { get; set; }

    [Serializable]  // 구조체는 인스펙터에서 안보이기에 Serializable로 해야 보이게 해줄수 있다.
    public struct ItemDropInfo
    {
        public ItemCode code;   //아이템 종류

        [Range(0, 1)]
        public float dropRatio; // 드랍 확률(1.0f -> 100%)
        public uint dropCount;  // 최대 드랍 개수
    }

    /// <summary>
    /// 이 적이 죽을 때 드랍하는 아이템 정보
    /// </summary>
    public ItemDropInfo[] dropItems;

    /// <summary>
    /// 상태별 업데이트 함수가 저장될 델리게이트(함수 저장용)
    /// </summary>
    Action onStateUpdate;

    // 컴포넌트들
    Animator animator;
    NavMeshAgent agent;
    SphereCollider bodyCollider;
    Rigidbody rigid;
    ParticleSystem dieEffect;
    EnemyHealthBar hpBar;

    int anim_MoveToHash = Animator.StringToHash("Move");
    int anim_StopToHash = Animator.StringToHash("Stop");
    int anim_AttackToHash = Animator.StringToHash("Attack");
    int anim_HitToHash = Animator.StringToHash("Hit");
    int anim_DieToHash = Animator.StringToHash("Die");

    // 읽기 전용
    readonly Vector3 EffectResetPosition = new(0.0f, 0.01f, 0.0f);
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        bodyCollider = GetComponent<SphereCollider>();
        rigid = GetComponent<Rigidbody>();
        Transform child = transform.GetChild(2);
        hpBar = child.GetComponent<EnemyHealthBar>();
        child = transform.GetChild(3);
        dieEffect = child.GetComponent<ParticleSystem>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        agent.speed = moveSpeed;
        rigid.isKinematic = true;  // 키네마틱을 꺼서 물리 작용 받기
        rigid.drag = float.MaxValue;            // 무한대로 되어있던 마찰력을 낮춰서 떨어질 수 있게 하기
        State = EnemyState.Wait;
        animator.ResetTrigger(anim_StopToHash); // Wait 상태로 설정하면서 Stop트리거가 쌓인것을 제거하기 위해 필요
        HP = maxHP;
    }

    protected override void OnDisable()
    {
        // 콜라이더 활성화

        bodyCollider.enabled = true;
        hpBar.gameObject.SetActive(true);      // hpBar 보이게 만들기
        agent.enabled = true;      // agent가 활성화 되어있으면 항상 네비메쉬 위에 있음

        base.OnDisable();
    }


    private void Update()
    {
        onStateUpdate();

    }
    
    /// <summary>
    /// wait 상태용 업데이트 함수
    /// </summary>
    void Update_Wait()
    {
        if (SearchPlayer())
        {
            State = EnemyState.Chase;
        }
        else
        {
            WaitTimer -= Time.deltaTime;    // 기다리는 시간 감소 0되면 Patrol로 변경

            // 다음 목적지를 바라보게 만들기
            Quaternion look = Quaternion.LookRotation(waypoints.NextTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 2);
        }

    }

    /// <summary>
    /// Patrol 상태용 업데이트 함수
    /// </summary>
    void Update_Patrol()
    {
        if (SearchPlayer())
        {
            State = EnemyState.Chase;
        }
        else if (agent.remainingDistance <= agent.stoppingDistance)   // 도착하면
        {
            waypoints.StepNextWaypoint();        // 웨이포인트 다음지점 설정하고
            State = EnemyState.Wait;             // wait 상태 변경
        }
    }
    void Update_Chase()
    {
        if (SearchPlayer())
        {
            agent.SetDestination(chaseTarget.position);

        }else
        {
            State = EnemyState.Wait;
        }
    }

    void Update_Attack()
    {

    }

    void Update_Dead()
    {

    }

    /// <summary>
    /// 시야 범위안에 플레이어가 있는지 없는지 찾는 함수
    /// </summary>
    /// <returns>찾았으면 true, 못찾았으면 false</returns>
    bool SearchPlayer()
    {
        bool result = false;
        chaseTarget = null;

        // 일정 반경(farSightRange)안에 있는 플레이어 찾기
        Collider[] colliders = Physics.OverlapSphere(transform.position, farSightRange, LayerMask.GetMask("Player"));
        Debug.Log(colliders.Length);
        if (colliders.Length > 0)
        {
            //일정 반경(farSightRange)안에 플레이어가 있다
            Vector3 playerPos = colliders[0].transform.position;   // 0번이 무조건 플레이어다
            Vector3 toPlayerDir = playerPos - transform.position;   // 적 -> 플레이어 방향벡터
            if (toPlayerDir.sqrMagnitude <= nearSightRange * nearSightRange+1f)  // 플레이어가 nearSightRange안쪽에 있다
            {
                chaseTarget = colliders[0].transform;
                result = true;
            }
            else
            {
                Debug.Log("A");
                Debug.Log(IsInSightAngle(toPlayerDir));
                Debug.Log(IsSightClear(toPlayerDir));
                if (IsInSightAngle(toPlayerDir) )   // 시야각안 인지 확인후 적과 플레이어 사이를 가리는 오브젝트가 있는지 확인
                {
                    if (IsSightClear(toPlayerDir))
                    {
                        chaseTarget = colliders[0].transform;
                        result = true;

                    }
                }
            }

        }


        return result;
    }

    /// <summary>
    /// 시야각(-sightHalfAngle ~ + sightHalfAngle)안에 플레이어가 있는지 없는지 확인하는 함수
    /// </summary>
    /// <param name="toTargetDirection">적에서 대상으로 향하는 방향 벡터</param>
    /// <returns>시야각 안에 있으면 true, 없으면 false</returns>
    bool IsInSightAngle(Vector3 toTargetDirection)
    {
        // 내적으로 각도 구해서 범위 설정
        // bool result = false;
        //float dot = Vector3.Dot(transform.forward.normalized, toTargetDirection.normalized);
        //float Angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        //if (Angle < sightHalfAngle)
        //{
        //    result = true;
        //    Debug.Log("시야각에 들어옴");
        //}

        float angle = Vector3.Angle(transform.forward, toTargetDirection);  // 적의 정면과 플레이어를 바라보는 각도 angle은 무조건 양수
      
        return sightHalfAngle > angle;
    }

    /// <summary>
    /// 적이 다른 오브젝트에 의해 가려지는지 아닌지 확인하는 함수
    /// </summary>
    /// <param name="toTargetDirection">적에서 대상으로 향하는 방향 벡터</param>
    /// <returns>true면 가려지지않음, false면 가려짐</returns>
    bool IsSightClear(Vector3 toTargetDirection)
    {
        bool result = false;
        Ray ray = new(transform.position + transform.up * 0.5f, toTargetDirection); // 레이생성 (눈 높이에 맞게 조금 높임)
        if (Physics.Raycast(ray, out RaycastHit hit, farSightRange))
        {
            if (hit.collider.CompareTag("Player"))  // 처음 충돌한게 플레이어면
            {
                result = true;                      // 플레이어 발견
            }
        }

        return result;
    }
    public void Attack(IBattler target)
    {
        target.Defence(AttackPower);
    }

    public void Defence(float damage)
    {
        if (IsAlive)
        {
            animator.SetTrigger(anim_HitToHash);
            HP -= Mathf.Max(0, damage - DefencePower);
            Debug.Log($"{HP}");
        }
    }

    public void Die()
    {
        Debug.Log("사망");
        State = EnemyState.Dead;
        StartCoroutine(DeadSquence());  // 사망연출 시작
        onDie?.Invoke();                // 죽었다고 알림
    }

    /// <summary>
    /// 사망 연출용 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator DeadSquence()
    {
        // 콜라이더 비활성화
        bodyCollider.enabled = false;

        // 사망 이펙트 처리
        dieEffect.Play();                       // 재생 시작
        dieEffect.transform.SetParent(null);    // 이펙트를 부모에서 분리해서 같이 안 가라앉게 만듬


        hpBar.gameObject.SetActive(false);      // hpBar 안보이게 만들기

        yield return new WaitForSeconds(0.5f); // 아이템이 바로 떨어지면 어색해서 약간 대기


        // 아이템 드랍
        MakeDropItems();

        // 사망 애니메이션 끝날때까지 대기
        yield return new WaitForSeconds(1.0f);

        // 바닥으로 가라 앉기 시작
        agent.enabled = false;      // agent가 활성화 되어있으면 항상 네비메쉬 위에 있음

        rigid.isKinematic = false;  // 키네마틱을 꺼서 물리 작용 받기
        rigid.drag = 10;            // 무한대로 되어있던 마찰력을 낮춰서 떨어질 수 있게 하기

        // 충분히 바닥아래로 내려갈때까지 대기
        yield return new WaitForSeconds(2.0f);

        // 슬라임 삭제
        dieEffect.transform.SetParent(this.transform);          // 부모 재설정
        dieEffect.transform.localPosition = EffectResetPosition;    // 위치 리셋

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 아이템을 드랍하는 함수
    /// </summary>
    void MakeDropItems()
    {
        foreach (var item in dropItems)
        {
            if (item.dropRatio > UnityEngine.Random.value)  // 확률 체크 후
            {
                uint count = (uint)UnityEngine.Random.Range(0, (int)item.dropCount) +1; // 개수 결정
                Factory.Instance.MakeItems(item.code, count,transform.position,true);   // 실제 생성
            }
        }
    }

    public void HealthRegenerate(float totalRegen, float duration)
    {
        throw new NotImplementedException();
    }

    public void HealthRegenerateByTick(float tickRegen, float tickInterval, uint totalTickCount)
    {
        throw new NotImplementedException();
    }


#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        bool playerShow = SearchPlayer();
        Handles.color = playerShow ? Color.red : Color.green;
        
        Vector3 forward = transform.forward * farSightRange;
        Handles.DrawDottedLine(transform.position, transform.position + forward, 2.0f);

        Quaternion q1 = Quaternion.AngleAxis(-sightHalfAngle, transform.up);
        Handles.DrawLine(transform.position, transform.position + q1 * forward);

        Quaternion q2 = Quaternion.AngleAxis(sightHalfAngle, transform.up);
        Handles.DrawLine(transform.position, transform.position + q2 * forward);

        Handles.DrawWireArc(transform.position, transform.up, q1 * forward, sightHalfAngle * 2,farSightRange,2.0f);

        Handles.DrawWireDisc(transform.position, transform.up, nearSightRange);


    }

#endif
}
