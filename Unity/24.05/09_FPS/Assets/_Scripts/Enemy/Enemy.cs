using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

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
        Idle = 0,   // 대기 상태, 가만히 서있는 형태
        Wander,     // 배회상태, 주변을 계속 왔다갔다한다.
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
                OnStateExit(state);
                state = value;
                Debug.Log($"{gameObject.name}_{state}");
                OnStateEnter(state);
            }
        }
    }

    /// <summary>
    /// 상태별 눈색
    /// </summary>
    [ColorUsage(true,true)]
    public Color[] stateEyeColors;

   

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

    /// <summary>
    /// 공격 범위 안에 플레이어가 들어왔는지 감지하는 센서
    /// </summary>
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

    /// <summary>
    /// Enenmy NavAgnent
    /// </summary>
    NavMeshAgent agent;

    /// <summary>
    /// 눈의 랜더러
    /// </summary>
    MeshRenderer eyeRenderer;

    /// <summary>
    /// 눈의 머티리얼
    /// </summary>
    Material eyeMaterial;

    /// <summary>
    /// 눈 색의 ID
    /// </summary>
    readonly int EyeColorID = Shader.PropertyToID("_EyeColor");

    // ------------------------------------------------------------------------------------------------------------------------


    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        SphereCollider sc = GetComponent<SphereCollider>();
        sc.radius = sightRange;

        attackSenser = GetComponentInChildren<AttackSenser>();
        attackSenser.onSensorTriggered += (target) =>
        {
            if (attackTarget == null)   // attack 상태에서 한번만 실행
            {
                attackTarget = target.GetComponent<Player>();
                attackTarget.onDie += ReturnWander;
                State = BehaviourState.Attack;

            }
        };

        Transform child = transform.GetChild(0);
        child = child.GetChild(1);
        child = child.GetChild(0);
        child = child.GetChild(0);
        eyeRenderer = child.GetComponent<MeshRenderer>();
        eyeMaterial = eyeRenderer.material;

        onUpdate = Update_Idle;
    }



    private void OnEnable()
    {
        
    }

    private void Update()
    {
        onUpdate();
    }

    void Update_Idle()
    {

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
        findTimeElapsed += Time.deltaTime;
        if (findTimeElapsed > findTime)
        {
            State = BehaviourState.Wander;  // 일정시간 동안 못찾으면 배회상태로 변경
        }
        else if (FindPlayer())
        {
            State = BehaviourState.Chase;
        }
    }

    void Update_Attack()
    {
        agent.SetDestination(attackTarget.transform.position);

        // 적이 플레이어를 바라보게 만들기
        Quaternion target = Quaternion.LookRotation(attackTarget.transform.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime);

        attackElapsed += Time.deltaTime;
        if (attackElapsed > attackInterval)
        {
            Attack();
            attackElapsed = 0;
        }



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
        eyeMaterial.SetColor(EyeColorID, stateEyeColors[(int)newState]);
        switch (newState)
        {
            case BehaviourState.Idle:
                onUpdate = Update_Idle;
                agent.speed = 0.0f;
                attackSenser.gameObject.SetActive(false);
                // 공격 정지
                break;

            case BehaviourState.Wander:
                onUpdate = Update_Wander;
                agent.speed = walkSpeed * (1-speedPenalty);
                agent.SetDestination(GetRandomDestination());
                break;
            case BehaviourState.Chase:
                onUpdate = Update_Chase;
                agent.speed = runSpeed * (1 - speedPenalty);

                break;
            case BehaviourState.Find:
                onUpdate = Update_Find;
                findTimeElapsed = 0.0f;
                agent.speed = walkSpeed * (1 - speedPenalty);
                agent.angularSpeed = 360.0f;
                StartCoroutine(LookAround());
                break;
            case BehaviourState.Attack:
                onUpdate = Update_Attack;
                break;
            case BehaviourState.Dead:
                DropItem();
                agent.speed = 0.0f;
                agent.velocity = Vector3.zero;
                onDie?.Invoke(this);            // 스포너에게 부활 요청용
                gameObject.SetActive(false);
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
            case BehaviourState.Idle:
                agent.speed = walkSpeed;
                attackSenser.gameObject.SetActive(true);
                break;
            case BehaviourState.Find:
                agent.angularSpeed = 120.0f;
                StopAllCoroutines();
                break;
            case BehaviourState.Attack:
                attackTarget.onDie -= ReturnWander;
                attackTarget = null;
                break;
            case BehaviourState.Dead:
                gameObject.SetActive(true);
                HP = maxHp;
                speedPenalty = 0.0f;
                attackPowerPenalty = 0.0f;  
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
        int range = 3;


        Vector2Int current = MazeVisualizer.WorldToGrid(transform.position);
        return MazeVisualizer.GridToWorld(UnityEngine.Random.Range(current.x - range,current.x + range +1), UnityEngine.Random.Range((current.y - range), current.y + range + 1));

    }

    /// <summary>
    /// 플레이어를 공격하는 함수
    /// </summary>
    void Attack()
    {
        attackTarget.OnAttacked(this);  // 피격 방향 표시를 위해 enemy 자체를 넘김
    }

    /// <summary>
    /// 공격 상태에서 배회 상태로 되돌리는 함수
    /// </summary>
    void ReturnWander()
    {
        State = BehaviourState.Wander;
    }

    /// <summary>
    /// 공격당함을 처리하는 함수
    /// </summary>
    /// <param name="hitLocation">공격당한 위치</param>
    /// <param name="damage">데미지</param>
    public void OnAttacked(HitLocation hit, float damage)
    {
        HP -= damage;
        switch (hit)
        {
            case HitLocation.Body:
                Debug.Log("몸통에 맞음");
                break;
            case HitLocation.Head:
                HP -= damage;
                Debug.Log("머리에 맞음");
                break;
            case HitLocation.Arm:
                attackPowerPenalty += 0.1f;
                Debug.Log("팔에 맞음");
                break;
            case HitLocation.Leg:
                speedPenalty += 0.3f;
                Debug.Log("다리 맞음");
                break;
        }

        if (State == BehaviourState.Wander || State == BehaviourState.Find) 
        {
            State = BehaviourState.Chase;
            agent.SetDestination(GameManager.Instance.Player.transform.position);
        }
        else if (State == BehaviourState.Chase)
        {
            agent.speed = runSpeed * (1 - speedPenalty);
        }

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
        Vector3[] positions =
        {         
            transform.position + transform.forward*1.5f, // 앞
            transform.position - transform.forward*1.5f, // 뒤
            transform.position + transform.right*1.5f,   // 오른쪽
            transform.position - transform.right*1.5f    // 왼쪽
        };

        int current;
        int prev = 0;
        int length = positions.Length;
        while (true)
        {
            do
            {
                current = UnityEngine.Random.Range(0, length);
            } while (current == prev);
            agent.SetDestination(positions[current]);
            prev = current;
            yield return new WaitForSeconds(1);
            Debug.Log(positions[prev]);
        }

    }

    /// <summary>
    /// 적을 리스폰 시키는 함수
    /// </summary>
    /// <param name="spawnPosition"></param>
    /// 
    public void Respawn(Vector3 spawnPosition, bool init = false)
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }
        agent.Warp(spawnPosition);
        if(init)
        {

            State = BehaviourState.Idle;
        }
        else
        {
            State = BehaviourState.Wander;
        }
    }

    /// <summary>
    /// 적을 움직이기 시작하게 만드는 함수
    /// </summary>
    public void Play()
    {
        State = BehaviourState.Wander;
    }

    /// <summary>
    /// 적을 멈추는 함수
    /// </summary>
    public void Stop()
    {
        State = BehaviourState.Idle;
    }

    /// <summary>
    /// 적이 드랍할 아이템의 종류를 나타내는 enum
    /// </summary>
    public enum ItemTable : byte
    {
        Heal,           // 힐 아이템
        AssaultRifle,   // 돌격소총
        Shotgun,        // 샷건
        Random          // 랜덤
    }

    /// <summary>
    /// 아이템을 드랍하는 함수
    /// </summary>
    /// <param name="table">드랍할 아이템</param>
    void DropItem(ItemTable table = ItemTable.Random)
    {
        ItemTable select = table;
        if (table == ItemTable.Random)
        {
            float random = UnityEngine.Random.value;
            if (random < 0.8f)
            {
                select = ItemTable.Heal;
            }
            else if (random < 0.9f)
            {
                select = ItemTable.AssaultRifle;
            }
            else
            {
                select =ItemTable.Shotgun;
            }
        }
        Factory.Instance.GetDropItem(select, transform.position);
    

        
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // 시야각 그리기(상태별로 시야각 다른 색으로 그리기)
        Color color = Color.red;
        Handles.color = color;
        if(chaseTarget != null)
        {
            Handles.DrawLine(transform.position + Vector3.up * 1.8f, chaseTarget.position);
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

    public void Test_StateChange(BehaviourState state)
    {
        State = state;
        agent.speed = 0;
        agent.velocity = Vector3.zero;
    }

    public void Test_EnemyStop()
    {
        agent.speed = 0;
        agent.velocity = Vector3.zero;
    }
#endif
}
