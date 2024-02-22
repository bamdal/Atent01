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
    Transform AttackSensorAxis;


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


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        inputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();

        currentSpeed = speed;
        AttackSensorAxis = transform.GetChild(0);
    }

    private void Update()
    {
        currentAttackCoolTime -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // 물리 프레임마다 inputDirection방향으로 초당 speed만큼 이동
        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * currentSpeed * inputDirection);
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
            AttackSensorAxis.rotation = Quaternion.identity;
        }
        else if(direction.y > 0.0f)
        {
            AttackSensorAxis.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (direction.x < 0.0f)
        {
            AttackSensorAxis.rotation = Quaternion.Euler(0, 0, -90);

        }
        else if(direction.x > 0.0f)
        {
            AttackSensorAxis.rotation = Quaternion.Euler(0, 0, 90);

        }
        else
        {
            AttackSensorAxis.rotation = Quaternion.identity;
        }
    }
}
// 플레이어가 공격하기
// - 애니메이션만 재생
// - 공격은 쿨타임이 있다.(1초)
// - 이동 중 공격을 하면 공격애니메이션 중에는 멈추고 애니메이션이 끝나면 다시 이동한다.
