using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetPlayer : NetworkBehaviour
{
    PlayerInputActions inputActions;
    CharacterController characterController;
    Animator animator;

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




    private void Awake()
    {
        inputActions = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        netAnimState.OnValueChanged += OnAnimStateChange;
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

        if (state != netAnimState.Value)
        {
            if(IsServer)
            {
                netAnimState.Value = state;
            }
            else if(IsOwner)
            {
                UpdateAnimStateServerRpc(state);

            }
        }

    }

    [ServerRpc]
    void UpdateAnimStateServerRpc(AnimationState state)
    {
        netAnimState.Value = state;
    }

    private void OnRotate(InputAction.CallbackContext context)
    {
        float rotateInput = context.ReadValue<float>();
        SetRotateInput(rotateInput * rotateSpeed);

        //rotate = rotateInput * rotateSpeed;
    }

    private void OnLClick(InputAction.CallbackContext context)
    {

    }

    private void OnRClick(InputAction.CallbackContext context)
    {

    }

    // --------------------------------------------------------------------------

    void SetMoveInput(float moveInput)
    {
        float moveDir = moveInput * moveSpeed;

        if (NetworkManager.Singleton.IsServer)
        {
            MoveRequestServerRpc(moveDir);

        }
        else if(IsOwner)
        {
            MoveRequestServerRpc(moveDir);
        }


    }

    /// <summary>
    /// 서버가 실행하는 함수임을 알리기 위해 ServerRpc어트리뷰트와 함수끝에 이름이 반드시 ServerRpc를 포함해야한다
    /// </summary>
    /// <param name="move"></param>
    [ServerRpc]
    void MoveRequestServerRpc(float move)
    {
        netMoveDir.Value = move;
    }


    private void SetRotateInput(float rotateInput)
    {
        if (NetworkManager.Singleton.IsServer )
        {
            RotateRequestServerRpc(rotateInput);
           

        }
        else if(IsOwner)
        {
            RotateRequestServerRpc(rotateInput);
           
        }
    }

    [ServerRpc]
    void RotateRequestServerRpc(float rotateInput)
    {
        netRotate.Value = rotateInput;

    }

    private void OnAnimStateChange(AnimationState previousValue, AnimationState newValue)
    {
        animator.SetTrigger(newValue.ToString());   
    }




}
