using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour, IAlive
{
    // 실습
    // 1. 플레이어는 WS키로 전진/후진을 한다.
    // 2. 플레이어는 AD키로 좌회전/우회전을 한다.
    // 3. 플레이어가 움직이면(전진/후진/좌회전/우회전) Player_Move애니메이션 재생된다
    // 4. 이동입력이 없으면 Player_Idle 애니메이션 재생
    // 5. Player_Move 애니메이션은 팔다리가 앞뒤로 흔들린다
    // 6. Player_Idle 애님이션은 머리가 살짝 앞뒤로 까닥거린다.

    PlayerInputActions inputActions;
    public Rigidbody rigid;
    Animator animator;

    /// <summary>
    /// 이동 방향(1 : 전진, -1 : 후진, 0 : 정지)
    /// </summary>
    float moveDirection = 0.0f;

    /// <summary>
    /// 이동 속도(기준 속도)
    /// </summary>
    public float moveSpeed = 5.0f;

    /// <summary>
    /// 현재 이동 속도
    /// </summary>
    float currentMoveSpeed = 5.0f;

    /// <summary>
    /// 회전 방향(1 : 우회전, -1 : 좌회전, 0 : 정지)
    /// </summary>
    float rotateDirection = 0.0f;

    /// <summary>
    /// 회전 속도
    /// </summary>
    public float rotateSpeed = 180.0f;

    /// <summary>
    /// 애니메이션 해쉬값
    /// </summary>
    readonly int IsMoveHash = Animator.StringToHash("IsMove");
    readonly int UseHash = Animator.StringToHash("Use");
    readonly int DieHash = Animator.StringToHash("Die");

    /// <summary>
    /// 점프력
    /// </summary>
    public float jumpPower = 6.0f;

    /// <summary>
    /// 공중에 떠 있는지 아닌지 나타내는 프로퍼티
    /// </summary>
    bool InAir
    {
        get => GroundCount < 1;
        
    }

    int groundCount = 0;
    /// <summary>
    /// 접촉하고 있는 "Ground" 태그 오브젝트의 개수
    /// </summary>
    int GroundCount
    {
        get => groundCount;
        set
        {
            if(groundCount < 0)
            {
                groundCount = 0;
            }
            groundCount = value;
            if (groundCount < 0)
            {
                groundCount = 0;
            }
            Debug.Log(groundCount);
        }
    }

    

    /// <summary>
    /// 점프 쿨 타임
    /// </summary>
    public float jumpCoolTime = 5.0f;

    /// <summary>
    /// 남아있는 쿨타임
    /// </summary>
    float jumpCoolRemains = -1.0f;

    /// <summary>
    /// 점프가 가능한지 확인하는 프로퍼티(점프중이 아니고 쿨타임이 아님)
    /// </summary>
    bool IsJumpAvailable => !InAir && (jumpCoolRemains < 0.0f);

    

    /// <summary>
    /// 사망 델리게이트
    /// </summary>
    public Action onDie;

    /// <summary>
    /// 시작했을때의 플레이어 수명
    /// </summary>
    public float startLifeTime = 10.0f;

    /// <summary>
    /// 현재 플레이어 수명
    /// </summary>
    float lifeTime = 0.0f;

    /// <summary>
    /// 플레이어의 수명을 확인하고 설정하기 위한 프로퍼티
    /// </summary>
    float LifeTime 
    { 
        get => lifeTime;
        set
        {
            lifeTime = value;
            if(lifeTime < 0.0f)
            {
                lifeTime = 0.0f;    // 수명이 다 되었으면 0.0으로 숫자 정리 및 사망처리
                Die();
            }
            onLifeTimeChange?.Invoke(lifeTime/startLifeTime);   // 현재 수명 비율 알림

        }
    }

    public Action<float> onLifeTimeChange;

    /// <summary>
    /// 플레이어 사망
    /// </summary>
    bool isAlive = true;
    private void Awake()
    {
        //inputActions = new PlayerInputActions();
        inputActions = new();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        ItemUseChecker checker = GetComponentInChildren<ItemUseChecker>();
        checker.onItemUse += (interacable) => interacable.Use();
   
    }

    private void Start()
    {
        currentMoveSpeed = moveSpeed;
        LifeTime = startLifeTime;
    }
    private void Update()
    {
        jumpCoolRemains -= Time.deltaTime;
        LifeTime -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Jump.performed += OnJumpInput;
        inputActions.Player.Use.performed += OnUseInput;
    }



    private void OnDisable()
    {
        inputActions.Player.Use.performed -= OnUseInput;
        inputActions.Player.Jump.performed -= OnJumpInput;
        inputActions.Player.Move.canceled -= OnMoveInput;
        inputActions.Player.Move.performed -= OnMoveInput;
        inputActions.Player.Disable();

    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
/*        context.started; // 살짝만 이라도 입력신호가 온경우
        context.performed; // 충분히 입력신호가 온경우
        context.canceled;*/
        SetInput(context.ReadValue<Vector2>(), !context.canceled);
        

    }

    private void OnJumpInput(InputAction.CallbackContext _)
    {
        Jump();
    }


    private void OnUseInput(InputAction.CallbackContext context)
    {
        animator.SetTrigger(UseHash);
    }

    /// <summary>
    /// 이동 입력 처리 함수
    /// </summary>
    /// <param name="input">입력된 방향</param>
    /// <param name="IsMove">이동중이면 true, 이동중이 아니면 false</param>
    void SetInput(Vector2 input, bool IsMove)
    {
        rotateDirection = input.x;
        moveDirection = input.y;
        animator.SetBool(IsMoveHash, IsMove);
    }

    /// <summary>
    /// 실제 이동 처리 함수(FixedUpdate에서 사용)
    /// </summary>
    void Move()
    {
        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * currentMoveSpeed * moveDirection * transform.forward);
    }

    /// <summary>
    /// 실제 회전 처리 함수(FixedUpdate에서 사용)
    /// </summary>
    void Rotate() 
    {
        // 회전을 표현하는 클래스 : Quaternion
        // Quaternion에 Quaternion을 곱하면 그만큼 회전함

        // Quaternion.Euler(); : x,y,z 축으로 얼마만큼 회전 시킬 것인지를 파라메터로 받어서 회전을 생성
        // Quaternion.AngleAxis(); : 특정 축을 기준으로 몇 도 만큼 회전 시킬지를 파라메터로 받아서 회전을 생성
        // Quaternion.FromToRotation() : 시작 방향에서 도착 방향이 될 수 있는 회전을 생성
        // Quaternion.Lerp(); : 일반 선형 보간, 시작 회전에서 목표 회전으로 보간하는 함수
        // Quaternion.Slerp(); : 시작 회전에서 목표 회전으로 보간하는 함수 (곡선으로 보간)
        // Quaternion.LookRotation() : 특정 방향을 바라보는 회전을 만들어주는 함수

        // Quaternion.identity; : 아무런 회전도 하지 않았다.
        // Quaternion.Inverse(); : 역회전을 계산하는 함수


        // Quaternion.RotateTowards(); : from에서 to로 회전 시키는 함수 한번 실행될때마다 maxDegressDelta만큼 회전 

        // 이번 fixedUpdate에서 추가로 회전할 회전각도
        Quaternion roate = Quaternion.AngleAxis(Time.fixedDeltaTime * rotateSpeed * rotateDirection, transform.up);
        
        // 현재 회전에서 rotate만큼 추가 회전 
        rigid.MoveRotation(rigid.rotation * roate);

        // transform.RotateAround : 특정 위치에서 특정 축을 기준으로 회전하기
    }


    /// <summary>
    /// 실제 점프를 처리하는 함수
    /// </summary>
    private void Jump()
    {
        if (IsJumpAvailable) // 점프가 가능할 때만 점프
        {
            rigid.AddForce(jumpPower * Vector3.up, ForceMode.Impulse);  // 위쪽으로 jumpPower만큼 힘을 더하기
            jumpCoolRemains = jumpCoolTime; // 쿨타임 초기화
            //GroundCount = 0;              // 점프했다고 표시
        }


    }

    /// <summary>
    /// 사망 처리용 함수
    /// </summary>
    public void Die()
    {
        if(isAlive)
        {
            Debug.Log("죽었음");
            animator.SetTrigger(DieHash);   // 사망 애니메이션

            inputActions.Player.Disable();  // 플레이어 조종 막기

            rigid.constraints = RigidbodyConstraints.None;  // 물리잠금 해제
            Transform head = transform.GetChild(0);
            rigid.AddForceAtPosition(-transform.forward, head.position, ForceMode.Impulse);
            rigid.AddTorque(transform.up * 1.5f, ForceMode.Impulse);

            onDie?.Invoke();
            isAlive = false;
        }
        // 죽는 애니메이션이 나온다.
        // 더이상 조정이 안되어야 한다.
        // 대굴대굴 구른다(뒤로 넘어가면서 y축으로 스핀 먹이기)
        // 죽었다고 신호보내기(onDie델리게이트 실행)
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {

            GroundCount++;

        }
        
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GroundCount--;


        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlatformBase platform = other.GetComponent<PlatformBase>();
        if(platform != null)
        {
            platform.onMove += OnRideMovingObject;  // 플랫폼 트리거에 들어갔을때 함수 연결
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlatformBase platform = other.GetComponent<PlatformBase>();
        if (platform != null)
        {
            platform.onMove -= OnRideMovingObject;  // 플랫폼 트리거에 나왔을때 연결 해제
        }
    }

    /// <summary>
    /// 움직이는 물체에 탑승했을 때 연결될 함수
    /// </summary>
    /// <param name="delta">움직인 양</param>
    void OnRideMovingObject(Vector3 delta)
    {
        rigid.MovePosition(rigid.position + delta);
    }

    /// <summary>
    /// 이동 속도 증감용 함수
    /// </summary>
    /// <param name="ratio">원본에서의 증감 비율</param>
    public void SetSpeedModifier(float ratio = 1.0f)
    {
        currentMoveSpeed = moveSpeed * ratio;
    }

    /// <summary>
    /// 원래 기준속도로 복구하는 함수
    /// </summary>
    public void RestoreMoveSpeed()
    {
        currentMoveSpeed = moveSpeed;
    }

}
// 실습
// 1. 점프 중에 점프가 되지 않아야 한다,
// 2. 점프 쿨타임이 있어야 한다.

