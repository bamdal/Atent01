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
    Vector3 dir = Vector3.zero;
    float speed=7.0f;
    Animator animator;
    private void Awake()
    {
        inputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.WASD.performed += Move;
        inputActions.Player.WASD.canceled += Move;
 
    }
    private void OnDisable()
    {
        inputActions.Player.WASD.canceled -= Move;
        inputActions.Player.WASD.performed -= Move;
        inputActions.Player.Disable();

    }

    private void Move(InputAction.CallbackContext context)
    {
        dir = context.ReadValue<Vector2>();
        if(context.performed) {
            animator.SetBool("IsMove", true);
        }
        if(context.canceled)
        {
            animator.SetBool("IsMove", false);

        }


    }

    private void FixedUpdate()
    {
        transform.Translate(Time.deltaTime * speed * new Vector3(0,0, dir.y));
        transform.Rotate(Time.deltaTime * speed*10.0f * new Vector3(0, dir.x, 0));
        //rb.MovePosition(rb.position + Time.deltaTime * speed * new Vector3(0, 0, dir.y));
    }
}
