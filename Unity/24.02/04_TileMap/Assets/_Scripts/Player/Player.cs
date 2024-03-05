using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // PlayerInputAction 만들기
    // Move 애니메이션 만들기
    // 10프레임 간격으로 0~3 반복
    // 2. Move Blend Tree만들기

    /// <summary>
    /// 이동 속도
    /// </summary>
    public float speed = 3.0f;

    float currentSpeed = 3.0f;

    /// <summary>
    /// 현재 입력된 이동 방향
    /// </summary>
    Vector2 inputDirection = Vector2.zero;

    /// <summary>
    /// 지금 움직이고 있는지 확인하는 변수(true면 움직인다)
    /// </summary>
    bool isMove = false;

    /// <summary>
    /// 공격 쿨타임
    /// </summary>
    public float attackCoolTime = 1.0f;

    /// <summary>
    /// 현재 남아있는 공격 쿨타임
    /// </summary>
    float currentAttackCoolTime;

    /// <summary>
    /// 공격 가능 여부
    /// </summary>
    public bool IsAttackReady => currentAttackCoolTime < 0.0f;



    /// <summary>
    /// AttackSensor 회전축
    /// </summary>
    Transform attackSensorAxis;

    /// <summary>
    /// 현재 내 공격 범위 안에 들어있는 적의 목록
    /// </summary>
    List<Slime> attackTargetList;

    /// <summary>
    /// 지금 공격이 유효한 상태인지 확인하는 변수
    /// </summary>
    bool isAttackVaild = false;

    /// <summary>
    /// 컴포넌트들
    /// </summary>
    Rigidbody2D rigid;
    PlayerInputActions inputActions;
    Animator animator;

    /// <summary>
    /// 애니메이션 해쉬값
    /// </summary>
    readonly int HashInputX = Animator.StringToHash("InputX");
    readonly int HashInputY = Animator.StringToHash("InputY");
    readonly int HashIsMove = Animator.StringToHash("IsMove");
    readonly int HashAttack = Animator.StringToHash("Attack");

    /// <summary>
    /// 플레이어가 현재 위치하고 있는 맵의 그리드
    /// </summary>
    Vector2Int currentMap;

    /// <summary>
    /// CurrentMap에 값을 설정할 떄 변경이 있으면 델리게이트를 실행해서 알리는 프로퍼티
    /// </summary>
    Vector2Int CurrentMap 
    {
        get => currentMap;
        set 
        {
            if (currentMap != value)
            {
                currentMap = value;
                onMapChange?.Invoke(currentMap);
            }

        }
        
    }


    /// <summary>
    /// 플레이어가 있는 맵이 변경되면 불리는 델리게이트
    /// </summary>
    public Action<Vector2Int> onMapChange;


    WorldManager world;

    /// <summary>
    /// 플레이어의 최대 수명
    /// </summary>
    public float maxLifeTime = 10.0f;

    /// <summary>
    /// 플레이어의 현재 수명
    /// </summary>
    float lifeTime;

    /// <summary>
    /// 수명을 확인하고 변경되었을 대의 처리를 하는 프로퍼티
    /// </summary>
    float LifeTime 
    {
        get => lifeTime;
        set 
        { 
            if(lifeTime != value)
            {
                lifeTime = value;   // 값을 설정

                lifeTime = Mathf.Clamp(lifeTime, 0.0f, maxLifeTime);    // 일정 범위를 벗어나지 않게함
                onLifeTimeChange?.Invoke(lifeTime/ maxLifeTime);        // 값이 변경되었음을 알림

            }
            
        }
    }


    /// <summary>
    /// 플레이어의 수명이 변경되었을 때 실행될 델리게이트(flaot : lifetime/maxlifetime)
    /// </summary>
    public Action<float> onLifeTimeChange;

    int killCount = 0;

    int KillCount
    {
        get => killCount;
        set
        {
            if (killCount != value)
            {
                killCount = value;
            }
        }
    }


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();

        currentSpeed = speed;
        attackSensorAxis = transform.GetChild(0);

        attackTargetList = new List<Slime>(4);
        AttackSensor sensor = attackSensorAxis.GetComponentInChildren<AttackSensor>();
        sensor.onEnemyEnter += (slime) =>   // 적이 센서 안에 들어오면
        {
            if (isAttackVaild)  // 공격이 유효한 상황이면
            {
                slime.Die();    // 즉시 죽이기
            }
            else
            {   // 공격이 유효하지 않으면
                 attackTargetList.Add(slime);    // 리스트에 추가하고
            }
            slime.ShowOutline(true);        // 아웃라인을 그린다
        };
        sensor.onEnemyExit += (slime) =>    // 적이 센서에서 나가면
        {
            attackTargetList.Remove(slime); // 리스트에서 제거하고
            slime.ShowOutline(false);       // 아웃라인을 끈다
        };

        LifeTime = maxLifeTime;
    }

    private void Start()
    {
        world = GameManager.Instance.World;
    }

    private void Update()
    {
        currentAttackCoolTime -= Time.deltaTime;
        LifeTime -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // 물리 프레임마다 inputDirection방향으로 초당 speed만큼 이동
        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * currentSpeed * inputDirection);
    
        CurrentMap = world.WorldToGrid(rigid.position); // 플레이어가 있는 맵 설정
    }
    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnStop;
        inputActions.Player.Attack.performed += OnAttack;
    }


    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Move.performed -= OnStop;
        inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // 입력값 받아와서

        inputDirection = context.ReadValue<Vector2>();

        // 애니메이션 조정
        animator.SetFloat(HashInputX, inputDirection.x);
        animator.SetFloat(HashInputY, inputDirection.y);
        isMove = true;
        animator.SetBool(HashIsMove, isMove);

        AttackSensorRotate(inputDirection);
    }
    private void OnStop(InputAction.CallbackContext context)
    {
        // 이동 방향을 0으로 만들고
        inputDirection = Vector2.zero;
        
        // InputX와 InputY를 변경하지 않는 이유
        // Idle애니메이션을 마지막 이동 방향으로 재생하기 위해
        isMove = false;
        animator.SetBool(HashIsMove, isMove);

    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        if(IsAttackReady)   // 공격 쿨타임이 다 되었으면
        {
            animator.SetTrigger(HashAttack);    // 애니메이션 재생
            currentAttackCoolTime = attackCoolTime; // 공격 쿨타임 초기화
            currentSpeed = 0.0f;
            isAttackVaild = false;      // 초기화(isAttackVaild 가 true로 고정되는 일 방지)
        }
    }

    /// <summary>
    /// 이동 속도를 원래대로 되돌리는 함수
    /// </summary>
    public void RestoreSpeed()
    {
        currentSpeed = speed;
    }

    /// <summary>
    /// 입력 방향에 따라 AttackSensor를 회전시키는 함수
    /// </summary>
    /// <param name="direction">입력 방향</param>
    void AttackSensorRotate(Vector2 direction)
    {
        // AttackSensorAxis.rotation = Quaternion.LookRotation(transform.forward,-direction);
        if(direction.y < 0.0f)
        {
            attackSensorAxis.rotation = Quaternion.identity;
        }
        else if(direction.y > 0.0f)
        {
            attackSensorAxis.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (direction.x < 0.0f)
        {
            attackSensorAxis.rotation = Quaternion.Euler(0, 0, -90);

        }
        else if(direction.x > 0.0f)
        {
            attackSensorAxis.rotation = Quaternion.Euler(0, 0, 90);

        }
        else
        {
            attackSensorAxis.rotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// 공격 애니메이션 진행 중에 공격이 유효해지면 애니메이션 이벤트로 실행
    /// </summary>
    void AttackValid()
    {
        isAttackVaild = true;
        foreach (var slime in attackTargetList)
        {
            slime.Die();
        }
        attackTargetList.Clear();
    }

    /// <summary>
    /// 공격 애니메이션 진행 중에 공격이 유효하지 않게 되면 애니메이션 이벤트로 실행
    /// </summary>
    void AttackNotVaild()
    {
        isAttackVaild = false;
    }

    /// <summary>
    /// 몬스터를 잡았을 때 실행할 함수
    /// </summary>
    /// <param name="bonus">몬스터 처리 보너스 (수명추가)</param>
    public void MonsterKill(float bonus)
    {
        LifeTime += bonus;
        killCount++;
    }
}
// 플레이어가 공격하기
// - 애니메이션만 재생
// - 공격은 쿨타임이 있다.(1초)
// - 이동 중 공격을 하면 공격애니메이션 중에는 멈추고 애니메이션이 끝나면 다시 이동한다.
