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

    PlayerInputActions inputActions;

    Animator animator;

    readonly int HashInputX = Animator.StringToHash("InputX");
    readonly int HashInputY = Animator.StringToHash("InputY");
    readonly int HashIsMove = Animator.StringToHash("IsMove");

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            animator.SetBool(HashIsMove, true);
            animator.SetFloat(HashInputX, context.ReadValue<Vector2>().x);
            animator.SetFloat(HashInputY, context.ReadValue<Vector2>().y);
        }
        if (context.canceled)
        {
            animator.SetBool(HashIsMove, false);
        }
    }
}
