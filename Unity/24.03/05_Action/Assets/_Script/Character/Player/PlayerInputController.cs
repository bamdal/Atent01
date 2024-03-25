using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{


    // 컴포넌트들
    PlayerInputActions inputActions;

    /// <summary>
    /// 이동 입력 전달하는 델리게이트(파라메터 : 이동방향,true면 눌렀다)
    /// </summary>
    public Action<Vector2,bool> onMove;

    /// <summary>
    /// 이동 모드 변경 입력을 전달하는 델리게이트
    /// </summary>
    public Action onMoveModeChange;

    /// <summary>
    /// 공격을 알리는 델리게이트
    /// </summary>
    public Action onAttack;

    public Action onItemPickup;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.MoveModeChange.performed += OnMoveModeChange;
        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Pickup.performed += OnPickup;

    }



    private void OnDisable()
    {
        inputActions.Player.Pickup.performed -= OnPickup;
        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.MoveModeChange.performed -= OnMoveModeChange;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Disable();
    }


    /// <summary>
    /// 이동 입력 처리용 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector3 input = context.ReadValue<Vector2>();
        onMove?.Invoke(input, !context.canceled);


    }


    /// <summary>
    /// 이동 모드 변경용 함수
    /// </summary>
    /// <param name="context"></param>
    private void OnMoveModeChange(InputAction.CallbackContext context)
    {
        onMoveModeChange?.Invoke();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        onAttack?.Invoke();
       
    }
    private void OnPickup(InputAction.CallbackContext context)
    {
        onItemPickup?.Invoke(); 
    }
}
