using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    // 실습
    // 1. 플레이어는 WS키로 전진/후진을 한다.
    // 2. 플레이어는 AD키로 좌회전/우회전을 한다.
    // 3. 플레이어가 움직이면(전진/후진/좌회전/우회전) Player_Move애니메이션 재생된다
    // 4. 이동입력이 없으면 Player_Idle 애니메이션 재생
    // 5. Player_Move 애니메이션은 팔다리가 앞뒤로 흔들린다
    // 6. Player_Idle 애님이션은 머리가 살짝 앞뒤로 까닥거린다.

    PlayerInputActions inputActions;
    Rigidbody rigid;
    Animator animator;


    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed = 5.0f;

    /// <summary>
    /// 회전 방향(1 : 우회전, -1 : 좌회전, 0 : 정지)
    /// </summary>
    float rotateDirection = 0.0f;

    /// <summary>
    /// 회전 속도
    /// </summary>
    public float rotateSpeed = 30.0f;

    /// <summary>
    /// 애니메이션 해쉬값
    /// </summary>
    readonly int IsMoveHash = Animator.StringToHash("IsMove");

    /// <summary>
    /// 점프력
    /// </summary>
    public float jumpPower = 6.0f;

    /// <summary>
    /// 점프 중인지 아닌지 나타내는 변수
    /// </summary>
    bool isJumping = false;

    /// <summary>
    /// 점프 쿨 타임
    /// </summary>
    public float jumpCoolTime = 5.0f;


    /// <summary>
    /// 점프가 가능한지 확인하는 프로퍼티(점프중이 아니고 쿨타임이 아님)
    /// </summary>
    bool IsJumpAvailable => !isJumping;

    /// <summary>
    /// 실제 이동방향을 나타낼 벡터
    /// </summary>
    Vector3 moveVector = Vector3.zero;

    /// <summary>
    /// 전진 명령
    /// </summary>
    float go = 0.0f;

    /// <summary>
    /// 우회전 명령
    /// </summary>
    float turnRight = 0.0f;

    CinemachineVirtualCamera virtualCamera;
    Vector3 cameraRotation;
    Vector3 cameraPosition;
    private void Awake()
    {
        //inputActions = new PlayerInputActions();
        inputActions = new();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        virtualCamera = transform.GetChild(3).GetComponent<CinemachineVirtualCamera>();
        cameraRotation = virtualCamera.transform.localRotation.eulerAngles;
        cameraPosition = virtualCamera.transform.localPosition;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Jump.performed += OnJumpInput;
        inputActions.Player.Dash.performed += OnDashInput;
        inputActions.Player.MouseVector.performed += OnMouseMove;
        inputActions.Player.MouseLC.performed += OnMouseLC;
        inputActions.Player.MouseLC.canceled += OnMouseLC; 
        inputActions.Player.MouseRC.performed += OnMouseRC;
        inputActions.Player.MouseRC.canceled += OnMouseRC;
    }



    private void OnDisable()
    {
        inputActions.Player.MouseRC.canceled -= OnMouseRC;
        inputActions.Player.MouseRC.performed -= OnMouseRC;
        inputActions.Player.MouseLC.canceled -= OnMouseLC;
        inputActions.Player.MouseLC.performed -= OnMouseLC;
        inputActions.Player.MouseVector.performed -= OnMouseMove;
        inputActions.Player.Dash.performed -= OnDashInput;
        inputActions.Player.Jump.performed -= OnJumpInput;
        inputActions.Player.Move.canceled -= OnMoveInput;
        inputActions.Player.Move.performed -= OnMoveInput;
        inputActions.Player.Disable();

    }


    private void OnMoveInput(InputAction.CallbackContext context)
    {

        SetInput(context.ReadValue<Vector2>(), !context.canceled);


    }

    private void OnJumpInput(InputAction.CallbackContext _)
    {
        Jump();
    }


    private void OnDashInput(InputAction.CallbackContext context)
    {

    }
    private void OnMouseRC(InputAction.CallbackContext context)
    {
    }

    private void OnMouseLC(InputAction.CallbackContext context)
    {
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        SetRotation(context.ReadValue<Vector2>());
    }

    void SetRotation(Vector2 input)
    {
        //rotateDirection = input.x;

        cameraRotation.y -= -input.x;

        virtualCamera.transform.localRotation = Quaternion.Euler(cameraRotation);
        virtualCamera.transform.localPosition = cameraPosition;
        //Rotate();
    }

    /// <summary>
    /// 이동 입력 처리 함수
    /// </summary>
    /// <param name="input">입력된 방향</param>
    /// <param name="IsMove">이동중이면 true, 이동중이 아니면 false</param>
    void SetInput(Vector2 input, bool IsMove)
    {
        go = input.y;
        turnRight = input.x;

        animator.SetBool(IsMoveHash, IsMove);
    }

    /// <summary>
    /// 실제 이동 처리 함수(FixedUpdate에서 사용)
    /// </summary>
    void Move()
    {
        moveVector = Vector3.zero;
        if (go > 0.0f)
            moveVector += transform.forward;
        else if (go < 0.0f)
            moveVector += -transform.forward;
        if (turnRight > 0.0f)
            moveVector += transform.right;
        else if(turnRight < 0.0f)
            moveVector += -transform.right;
        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * moveSpeed * moveVector);
    }

    /// <summary>
    /// 실제 회전 처리 함수(FixedUpdate에서 사용)
    /// </summary>
    void Rotate()
    {

        // 이번 fixedUpdate에서 추가로 회전할 회전각도
        Quaternion roate = Quaternion.AngleAxis(Time.fixedDeltaTime * rotateSpeed * rotateDirection, transform.up);

        // 현재 회전에서 rotate만큼 추가 회전 
        rigid.MoveRotation(rigid.rotation * roate);
    }


    /// <summary>
    /// 실제 점프를 처리하는 함수
    /// </summary>
    private void Jump()
    {
        if (IsJumpAvailable) // 점프가 가능할 때만 점프
        {
            rigid.AddForce(jumpPower * Vector3.up, ForceMode.Impulse);  // 위쪽으로 jumpPower만큼 힘을 더하기
            isJumping = true;               // 점프했다고 표시
        }


    }


    private void FixedUpdate()
    {
        Move();
        //Rotate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;

        }

    }

}
