using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
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
    /// 마지막 입력으로 인한 이동 방향(전진, 정지, 후진)
    /// </summary>
    float moveDir = 0.0f;

    /// <summary>
    /// 마지막 입력으로 인한 회전 방향(좌회전, 정지, 우회전)
    /// </summary>
    float rotate = 0.0f;

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
    /// 애니메이션 상태 설정 및 확인용 프로퍼티
    /// </summary>
    AnimationState State
    {
        get => state;
        set
        {
            if (value != state)
            {
                state = value;
                animator.SetTrigger(state.ToString());
            }
        }
    }


    private void Awake()
    {
        inputActions = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
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
        characterController.SimpleMove(moveDir * transform.forward);
        transform.Rotate(0,rotate * Time.deltaTime,0,Space.World);
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        float moveInput = context.ReadValue<float>(); // -1,0,1 중 하나
        moveDir = moveInput * moveSpeed;
        if(moveDir > 0.001f)
        {
            State = AnimationState.Walk;
        }
        else if (moveDir < -0.001f)
        {
            State = AnimationState.BackWalk;
        }
        else
        {
            State = AnimationState.Idle;
        }
    }


    private void OnRotate(InputAction.CallbackContext context)
    {
        float rotateInput = context.ReadValue<float>();
        rotate = rotateInput * rotateSpeed;
    }



    private void OnLClick(InputAction.CallbackContext context)
    {
        
    }

    private void OnRClick(InputAction.CallbackContext context)
    {
       
    }


}
