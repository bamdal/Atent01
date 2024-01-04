using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


[RequireComponent(typeof(Animator))]

public class Player : MonoBehaviour
{

    InputSystems inputSystems;
    Vector3 Inputdir= Vector3.zero;
    public float speed = 6.0f;
    Animator anim;
    readonly int InputX_String = Animator.StringToHash("DirectionX");
    readonly int InputY_String = Animator.StringToHash("DirectionY");
    readonly int Input_Move = Animator.StringToHash("IsMoving");
    private void Awake()
    {
        inputSystems = new InputSystems();
        anim = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        inputSystems.Player.Enable();
        inputSystems.Player.Move.performed += OnMove;
        inputSystems.Player.Move.canceled += OnMove;
        inputSystems.Player.Dash.performed += OnDash;
        inputSystems.Player.Dash.canceled += OnDash;
        inputSystems.Player.LClick.performed += OnLClick;
        inputSystems.Player.LClick.canceled += OnLClick;
        inputSystems.Player.RClick.performed += OnRClick;
        inputSystems.Player.RClick.canceled += OnRClick;
    }

    private void OnDisable()
    {
        inputSystems.Player.RClick.canceled -= OnRClick;
        inputSystems.Player.RClick.performed -= OnRClick;
        inputSystems.Player.LClick.canceled -= OnLClick;
        inputSystems.Player.LClick.performed -= OnLClick;
        inputSystems.Player.Dash.canceled -= OnDash;
        inputSystems.Player.Dash.performed -= OnDash;
        inputSystems.Player.Move.canceled -= OnMove;
        inputSystems.Player.Move.performed -= OnMove;
        inputSystems.Player.Disable();
    }

    private void OnRClick(InputAction.CallbackContext context)
    {
        
    }

    private void OnLClick(InputAction.CallbackContext context)
    {
        
    }



    private void OnDash(InputAction.CallbackContext context)
    {
        
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if(context.performed) 
        {
            Inputdir = context.ReadValue<Vector2>();
            anim.SetFloat(InputX_String, Inputdir.x);
            anim.SetFloat(InputY_String, Inputdir.y);
           
            anim.SetBool(Input_Move, true);
        }
        if(context.canceled)
        {
            Inputdir = context.ReadValue<Vector2>();
            anim.SetFloat(InputX_String, Inputdir.x);
            anim.SetFloat(InputY_String, Inputdir.y);
            anim.SetBool(Input_Move, false);
        }

        
    }

    private void Update()
    {
        transform.Translate(Time.deltaTime*speed*Inputdir);
    }


}
