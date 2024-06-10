using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

/// <summary>
/// 적이 맞을 수 있는 부위
/// </summary>
public enum HitLocation : byte
{
    Body,
    Head,
    Arm,
    Leg
}

public class Enemy : MonoBehaviour
{
    // 상태 관련 --------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 적의 상태
    /// </summary>
    public enum BehaviourState : byte
    {
        Wander = 0, // 배회상태, 주변을 계속 왔다갔다한다.
        Chase,      // 추적상태, 플레이어가 마지막으로 목격된장소를 향해 계속 이동한다.
        Find,       // 탐색상태, 추적 도주에 플레이어가 시야에서 사라지면 주변을 두리번 거리며 찾는다
        Attack,     // 공격상태, 추적 상태일때 플레이어가 일정범위안에 들어오면 일정주기로 공격
        Dead        // 사망상태, 죽어서 디스폰(일정 시간 후에 재생성)
    }

    /// <summary>
    /// 적의 현재 상태
    /// </summary>
    BehaviourState state = BehaviourState.Dead;

    /// <summary>
    /// 적의 상태 확인 및 설정용 프로퍼티
    /// </summary>
    BehaviourState State
    {
        get => state;
        set
        {
            if (state != value)
            {
                OnStateExit(value);
                state = value;
                Debug.Log(state);
                OnStateEnter(value);
            }
        }
    }

    /// <summary>
    /// 각 상태가 되었을 때 상태별 업데이트함수를 저장하는 델리게이트
    /// </summary>
    Action onUpdate = null;

    // HP 관련 --------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 최대 HP
    /// </summary>
    public float maxHp = 30.0f;

    /// <summary>
    /// 현재 HP
    /// </summary>
    [SerializeField]
    float hp = 30.0f;

    /// <summary>
    /// HP 설정 및 확인용 프로퍼티
    /// </summary>
    public float HP
    {
        get => hp;
        set 
        {
            hp = value; 
            if (hp <= 0)
            {
                State = BehaviourState.Dead;    // HP가 0이하면 사망
            }
        }
    }

    /// <summary>
    /// 사망시 시작될 델리게이트
    /// </summary>
    public Action<Enemy> onDie;

    // 이동 관련------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 이동속도(배회, 찾기 상태일때 적용)
    /// </summary>
    public float walkSpeed = 2.0f;

    /// <summary>
    /// 이동속도(추적 및 공격 상태일때 적용)
    /// </summary>
    public float runSpeed = 7.0f;

    /// <summary>
    /// 이동 패널티(다리를 맞으면 증가)
    /// </summary>
    float speedPenalty = 0;

    // 시야 관련------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// 적의 시야각
    /// </summary>
    public float sightAngle = 90.0f;

    /// <summary>
    /// 적이 시야 범위
    /// </summary>
    public float sightRange = 20.0f;

    AttackSenser attackSenser;

    // 공격 관련------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// 공격할 대상
    /// </summary>
    Player attackTarget = null;

    /// <summary>
    /// 공격력
    /// </summary>
    public float attackPower = 10.0f;

    /// <summary>
    /// 공격 속도
    /// </summary>
    public float attackInterval = 1.0f;

    /// <summary>
    /// 공격 시간 측정용
    /// </summary>
    float attackElapsed = 0;

    /// <summary>
    /// 팔 부상시 누적될 공격력 패널티
    /// </summary>
    float attackPowerPenalty = 0;

    // 탐색 관련------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 탐색 상태에서 배회 상태로 돌아가기까지 걸리는 시간
    /// </summary>
    public float findTime = 5.0f;

    /// <summary>
    /// 탐색 진행 시간
    /// </summary>
    float findTimeElapsed = 0.0f;

    /// <summary>
    /// 추적 대상
    /// </summary>
    Transform chaseTarget = null;

    // 기타 ------------------------------------------------------------------------------------------------------------------------

    NavMeshAgent agent;

    // ------------------------------------------------------------------------------------------------------------------------


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        SphereCollider sc = GetComponent<SphereCollider>();
        sc.radius = sightRange;

        hp = maxHp;

        attackSenser = GetComponentInChildren<AttackSenser>();
        attackSenser.onSensorTriggered += OnSensorTriggered;
    }

    private void OnEnable()
    {
        
    }

    private void Update()
    {
        onUpdate();
    }


    void Update_Wander()
    {
        // 플레이어를 찾으면 Chase 상태로 변경하기
        if (FindPlayer())
        {
            State = BehaviourState.Chase;
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {

            agent.SetDestination(GetRandomDestination());
        }

    }
    void Update_Chase()
    {
        // 마지막 목격 장소까지 이동하기
        if (IsPlayerInSight(out Vector3 position))
        {
            agent.SetDestination(position); // 마지막 목격 장소를 목적지로 새로 설정
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            State = BehaviourState.Find;
        }
    }

    void Update_Find()
    {
        if (IsPlayerInSight(out Vector3 position))
        {
            StopCoroutine(goWander);
            State = BehaviourState.Chase;
        }
        else if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if(wait)
                goWander = StartCoroutine(GoWander());
            transform.Rotate(Time.deltaTime*360f * Vector3.up);
        }
    }
    Coroutine goWander;
    bool wait = true;
    IEnumerator GoWander()
    {
        wait = false;
        yield return new WaitForSeconds(3.0f);
        State = BehaviourState.Wander;
        wait = true;
    }

    void Update_Attack()
    {
        Debug.Log("공격중");
    }

    void Update_Dead()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            chaseTarget = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            chaseTarget = null;
        }
    }

    /// <summary>
    /// 특정 상태가 되었을 때의 처리를 실행하는 함수
    /// </summary>
    /// <param name="newState">새 상태</param>
    void OnStateEnter(BehaviourState newState)
    {
        switch (newState)
        {
            case BehaviourState.Wander:
                onUpdate = Update_Wander;
                agent.speed = walkSpeed;
                agent.SetDestination(GetRandomDestination());
                break;
            case BehaviourState.Chase:
                onUpdate = Update_Chase;
                agent.speed = runSpeed;
                break;
            case BehaviourState.Find:
                onUpdate = Update_Find;
                break;
            case BehaviourState.Attack:
                onUpdate = Update_Attack;
                break;
            case BehaviourState.Dead:
                onUpdate = Update_Dead;
                break;
        }
    }

    /// <summary>
    /// 특정 상태에서 나갔을때의 처리를 실행하는 함수
    /// </summary>
    /// <param name="newState">새 상태</param>
    void OnStateExit(BehaviourState newState)
    {
        switch (newState)
        {

            case BehaviourState.Find:
                break;
            case BehaviourState.Attack:
                break;
            case BehaviourState.Dead:
                gameObject.SetActive(false);
                break;
            default:
            //case BehaviourState.Wander:
            //case BehaviourState.Chase:
                break;
        }
    }

    /// <summary>
    /// 배회하기 위해 랜덤한 위치를 돌려주는 함수
    /// </summary>
    /// <returns>랜덤한 배회용 목적지</returns>
    Vector3 GetRandomDestination()
    {

        /*NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position + UnityEngine.Random.insideUnitSphere * sightRange, out hit, sightRange, NavMesh.AllAreas))
        {
            result = hit.position;
        }
*/
        float size = CellVisualizer.CellSize;
        int range = 3;


        Vector2Int current = MazeVisualizer.WorldToGrid(transform.position);
        return MazeVisualizer.GridToWorld(UnityEngine.Random.Range(current.x - range,current.x + range +1), UnityEngine.Random.Range((current.y - range), current.y + range + 1));

    }

    /// <summary>
    /// 플레이어를 공격하는 함수
    /// </summary>
    void Attack()
    {

    }

    /// <summary>
    /// 공격당함을 처리하는 함수
    /// </summary>
    /// <param name="hitLocation">공격당한 위치</param>
    /// <param name="damage">데미지</param>
    public void OnAttacked(HitLocation hitLocation, float damage)
    {

    }

    /// <summary>
    /// 플레이어를 찾는 시도를 함수
    /// </summary>
    /// <returns>true면 찾음, false면 못찾음</returns>
    bool FindPlayer()
    {
        bool result = false;
        if (chaseTarget != null)               // 추적 대상이 존재하고
        {
            result = IsPlayerInSight(out _);    // 시야 범위 안에있으면 플레이어를 찾은 것
        }
        return result;
    }

    /// <summary>
    /// 플레이어가 시야범위 안에 있는지 확인하는 함수
    /// </summary>
    /// <param name="position">플레이어가 시야범위 안에 있을 때 플레이어의 위치</param>
    /// <returns>true면 시야범위내에 있다, false면 없다</returns>
    bool IsPlayerInSight(out Vector3 position)
    {
        bool result = false;
        position = Vector3.zero;
        if(chaseTarget != null)
        {
            Vector3 dir = chaseTarget.position - transform.position;
            Ray ray = new Ray(transform.position+Vector3.up*1.8f, dir); // 적 눈에서 나오는 ray
            if (Physics.Raycast(ray, out RaycastHit hit, sightRange, LayerMask.GetMask("Player", "Wall")))
            {
                if (hit.transform == chaseTarget)   // 플레이어를 발견
                {
                    float angle = Vector3.Angle(transform.forward, dir);
                    if (angle * 2 < sightAngle)
                    {
                        position = chaseTarget.position;
                        result = true;
                    }
                }
            }
            
        }

        return result;
    }

    /// <summary>
    /// 주위를 두리번 거리는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator LookAround()
    {
        yield return null;
    }

    /// <summary>
    /// 적을 리스폰 시키는 함수
    /// </summary>
    /// <param name="spawnPosition"></param>
    public void Respawn(Vector3 spawnPosition)
    {
        agent.Warp(spawnPosition);
        State = BehaviourState.Wander;
    }

    /// <summary>
    /// 적이 드랍할 아이템의 종류를 나타내는 enum
    /// </summary>
    enum ItemTable : byte
    {
        Heal,           // 힐 아이템
        AssaultRifle,   // 돌격소총
        Shotgun,        // 샷건
        Random          // 랜덤
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="table">드랍할 아이템</param>
    void DropItem(ItemTable table = ItemTable.Random)
    {

    }
    private void OnSensorTriggered(GameObject obj)
    {
        Player player = obj.GetComponent<Player>();
        if (player != null)
        {
            State = BehaviourState.Attack;
        }

    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // 시야각 그리기(상태별로 시야각 다른 색으로 그리기)
        Color color = Color.red;
        Handles.color = color;
        if(chaseTarget != null)
        {
            Vector3 dir = chaseTarget.position - transform.position;
            Handles.DrawLine(transform.position + Vector3.up * 1.8f, dir);
        }

        color = Color.white;
        Handles.color = color;
        Handles.DrawWireDisc(transform.position, transform.up, sightRange);

        switch (State)
        {
            case BehaviourState.Wander:
                color = Color.blue;
                break;
            case BehaviourState.Chase:
                color = Color.yellow;
                break;
            case BehaviourState.Find:
                color = Color.green;
                break;
            case BehaviourState.Attack:
                color = Color.red;
                break;
            case BehaviourState.Dead:
                color = Color.gray;
                break;
        }

        Handles.color = color;
        Handles.DrawWireArc(transform.position, transform.up, transform.forward*sightRange, sightAngle/2, sightRange,3.0f);
        Handles.DrawWireArc(transform.position, transform.up, transform.forward*sightRange, -sightAngle/2, sightRange,3.0f);
    }

    public Vector3 Test_GetRandomPosition()
    {
        return GetRandomDestination();
    }

#endif
}
