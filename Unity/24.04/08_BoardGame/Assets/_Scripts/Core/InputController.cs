using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 입력을 받으면 델리게이트로 알리는 역할만 하는 클래스
/// </summary>
public class InputController : MonoBehaviour
{
    public Action<Vector2> onMouseMove;
    public Action<Vector2> onMouseClick;
    public Action<float> onMouseWheel;

    PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Click.performed += OnClick;
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Wheel.performed += OnWheel;
    }

    private void OnDisable()
    {
        inputActions.Player.Wheel.performed -= OnWheel;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Click.performed -= OnClick;
        inputActions.Player.Disable();
    }

    private void OnWheel(InputAction.CallbackContext context)
    {
        onMouseWheel?.Invoke(context.ReadValue<float>());
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // 어디로 움직였는지 알림
        onMouseMove?.Invoke(context.ReadValue<Vector2>());
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        // 어느 위치를 클랙했는지 알림
        onMouseClick?.Invoke(Mouse.current.position.ReadValue());
    }

    /// <summary>
    /// 기존에 바인딩 된 함수들을 제거
    /// </summary>
    public void ResetBind()
    {
        onMouseMove = null;
        onMouseClick = null;
        onMouseWheel = null;
    }
}
