using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetPlayer : NetworkBehaviour
{

    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed = 3.5f;

    /// <summary>
    /// 회전 속도
    /// </summary>
    public float rotateSpeed = 90.0f;


    /// <summary>
    /// 마지막 입력으로 인한 이동 방향(전진, 정지, 후진), 네트워크에서 공유되는 변수
    /// </summary>
    NetworkVariable<float> netMoveDir = new NetworkVariable<float>(0.0f);

    /// <summary>
    /// 마지막 입력으로 인한 회전 방향(좌회전, 정지, 우회전)
    /// </summary>
    NetworkVariable<float> netRotate = new NetworkVariable<float>(0.0f);

    /// <summary>
    /// 애니메이션 상태
    /// </summary>
    enum AnimationState
    {
        Idle,       // 대기
        Walk,       // 걷기
        BackWalk,   // 뒤로 걷기
        None        // 초기값
    }

    /// <summary>
    /// 현재 애니메이션 상태
    /// </summary>
    AnimationState state = AnimationState.None;

    /// <summary>
    /// 애니메이션 네트워크용 변수
    /// </summary>
    NetworkVariable<AnimationState> netAnimState = new NetworkVariable<AnimationState>();

    /// <summary>
    /// 채팅용 네트워크 변수
    /// </summary>
    NetworkVariable<FixedString512Bytes> chatString = new NetworkVariable<FixedString512Bytes> ();

    // 컴포넌트들
    PlayerInputActions inputActions;
    CharacterController characterController;
    Animator animator;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        netAnimState.OnValueChanged += OnAnimStateChange;
        chatString.OnValueChanged += OnChatRecive;
    }



    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.MoveForward.performed += OnMoveInput;
        inputActions.Player.MoveForward.canceled += OnMoveInput;
        inputActions.Player.Rotate.performed += OnRotate;
        inputActions.Player.Rotate.canceled += OnRotate;
        inputActions.Player.AttackLC.performed += OnLClick;
        inputActions.Player.AttackRC.performed += OnRClick;

    }



    private void OnDisable()
    {
        inputActions.Player.AttackRC.performed -= OnRClick;
        inputActions.Player.AttackLC.performed -= OnLClick;
        inputActions.Player.Rotate.canceled -= OnRotate;
        inputActions.Player.Rotate.performed -= OnRotate;
        inputActions.Player.MoveForward.canceled -= OnMoveInput;
        inputActions.Player.MoveForward.performed -= OnMoveInput;
        inputActions.Disable();
    }




    // Update is called once per frame
    void Update()
    {
        if (netMoveDir.Value != 0)
        {
            characterController.SimpleMove(netMoveDir.Value * transform.forward);
        }
        if(netRotate.Value != 0)
        {
            transform.Rotate(0, netRotate.Value * Time.deltaTime, 0, Space.World);

        }
    }

    // 입력처리용 함수 ---------------------------------------------------------------------
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        float moveInput = context.ReadValue<float>(); // -1,0,1 중 하나
        SetMoveInput(moveInput);
    }



    private void OnRotate(InputAction.CallbackContext context)
    {
        float rotateInput = context.ReadValue<float>();
        SetRotateInput(rotateInput);

        //rotate = rotateInput * rotateSpeed;
    }

    private void OnLClick(InputAction.CallbackContext context)
    {

    }

    private void OnRClick(InputAction.CallbackContext context)
    {

    }

    // --------------------------------------------------------------------------

    /// <summary>
    /// 이동 입력 처리함수
    /// </summary>
    /// <param name="moveInput">이동 입력된 정도</param>
    void SetMoveInput(float moveInput)
    {

        if (IsOwner)    // 오너 일때만 이동처리
        {
            float moveDir = moveInput * moveSpeed;  // 이동 정도 결정

            if (IsServer)                           // 서버면 직접 수정
            {
                netMoveDir.Value = moveDir;

            }
            else
            {
                MoveRequestServerRpc(moveDir);      // 서버가 아니라면 서버에게 수정 요청하기

            }
            
            // 애니메이션 변경
            if (moveInput > 0.001f)
            {
                state = AnimationState.Walk;
            }
            else if (moveInput < -0.001f)
            {
                state = AnimationState.BackWalk;
            }
            else
            {
                state = AnimationState.Idle;
            }

            if (state != netAnimState.Value)    // 애니메이션 상태가 변화했다면
            {
                if (IsServer)                   // 서버면 직접 수정
                {
                    netAnimState.Value = state;
                }
                else if (IsOwner)               // 아니면 서버에 요청
                {
                    UpdateAnimStateServerRpc(state);

                }
            }

        }

    }

    /// <summary>
    /// 회전 입력처리 함수
    /// </summary>
    /// <param name="rotateInput">회전 입력 정도</param>
    private void SetRotateInput(float rotateInput)
    {
        if (IsOwner)    // 오너일 때만 처리
        {
            float rotate = rotateInput * rotateSpeed;   // 회전량 연산
            if (NetworkManager.Singleton.IsServer)
            {
                netRotate.Value = rotate;


            }
            else
            {
                RotateRequestServerRpc(rotate);

            }
        }

    }


    private void OnAnimStateChange(AnimationState previousValue, AnimationState newValue)
    {
        animator.SetTrigger(newValue.ToString());
    }

    // 채팅 --------------------------------------------------------------------------------------------------

    /// <summary>
    /// 채팅을 보내는 함수
    /// </summary>
    /// <param name="message"></param>
    public void SendChat(string message)
    {
        // chatString 변경
        if (IsServer)
        {
            chatString.Value = message;

        }
        else if (IsOwner)
        {
            RequestChatServerRpc(message);
        }

    }

    /// <summary>
    /// 채팅을 받았을 때 처리하는 함수(chatString이 변경되었으면 호출)
    /// </summary>
    /// <param name="previousValue"></param>
    /// <param name="newValue"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnChatRecive(FixedString512Bytes previousValue, FixedString512Bytes newValue)
    {
        GameManager.Instance.Log(newValue.ToString());  // 받은 내용 logger에 찍기
    }



    // 서버 Rpc들 -----------------------------------------------------------------------------------------

    /// <summary>
    /// 서버가 실행하는 함수임을 알리기 위해 ServerRpc어트리뷰트와 함수끝에 이름이 반드시 ServerRpc를 포함해야한다
    /// </summary>
    /// <param name="move"></param>
    [ServerRpc]
    void MoveRequestServerRpc(float move)
    {
        netMoveDir.Value = move;
    }

    [ServerRpc]
    void UpdateAnimStateServerRpc(AnimationState state)
    {
        netAnimState.Value = state;
    }

    [ServerRpc]
    void RotateRequestServerRpc(float rotateInput)
    {
        netRotate.Value = rotateInput;

    }

    [ServerRpc]
    void RequestChatServerRpc(FixedString512Bytes message)
    {
        chatString.Value = message;
    }




}
