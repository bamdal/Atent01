using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IBattler, IHealth
{
    protected enum EnemyState
    {
        Wait = 0,   // 대기
        Patrol,     // 순찰
        Chase,      // 추적
        Attack,     // 공격
        Dead        // 사망
    }

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
                        Debug.Log("대기");
                        agent.isStopped = true;                 // agent 정지 
                        agent.velocity = Vector3.zero;          // agnet에 남아있던 운동량 제거
                        animator.SetTrigger(anim_StopToHash);   // 애니메이션 정지
                        WaitTimer = waitTime;                   // 기다려야 하는 시간 초기화
                        onStateUpdate = Update_Wait;            // 대기 상태용 업데이트 함수
                        break;
                    case EnemyState.Patrol:
                        Debug.Log("패트롤");
                        agent.isStopped = false;
                        agent.destination = waypointTarget.position;
                        animator.SetTrigger(anim_MoveToHash);
                        onStateUpdate = Update_Patrol;
                        break;
                    case EnemyState.Chase:
                        onStateUpdate = Update_Chase;
                        break;
                    case EnemyState.Attack:
                        onStateUpdate = Update_Attack;
                        break;
                    case EnemyState.Dead:
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
    /// 적이 순찰할 웨이포인트
    /// </summary>
    public Waypoints waypoints;

    /// <summary>
    /// 이동할 웨이포인트의 트랜스폼
    /// </summary>
    protected Transform waypointTarget = null;

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
    public Action<float> onHealthChange { get ; set ; }

    
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

        [Range(0,1)]
        public float dropRatio; // 드랍 확률(1.0f -> 100%)
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

    int anim_MoveToHash = Animator.StringToHash("Move");
    int anim_StopToHash = Animator.StringToHash("Stop");
    int anim_AttackToHash = Animator.StringToHash("Attack");
    int anim_HitToHash = Animator.StringToHash("Hit");
    int anim_DieToHash = Animator.StringToHash("Die");


    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        bodyCollider = GetComponent<SphereCollider>();
        rigid = GetComponent<Rigidbody>();
        //dieEffect = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        agent.speed = moveSpeed;
        if (waypoints == null)
        {
            waypointTarget = transform;
        }
        else
        {
            waypointTarget = waypoints.Current;
        }

        State = EnemyState.Wait;
    }

    private void Update()
    {
        onStateUpdate();


    }

    void Update_Wait()
    {
        WaitTimer -= Time.deltaTime;
    }

    void Update_Patrol()
    {
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            State = EnemyState.Wait;
            waypointTarget = waypoints.Current;
        }
    }
    void Update_Chase()
    {

    }

    void Update_Attack()
    {

    }

    void Update_Dead()
    {

    }

    public void Attack(IBattler target)
    {
        throw new System.NotImplementedException();
    }

    public void Defence(float damage)
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        throw new NotImplementedException();
    }

    public void HealthRegenerate(float totalRegen, float duration)
    {
        throw new NotImplementedException();
    }

    public void HealthRegenerateByTick(float tickRegen, float tickInterval, uint totalTickCount)
    {
        throw new NotImplementedException();
    }
}
