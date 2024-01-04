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
    Vector3 InputMouse = Vector3.zero;
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
        inputSystems.Player.MousePostion.performed += OnMousePostion;
    }



    private void OnDisable()
    {
        inputSystems.Player.MousePostion.performed -= OnMousePostion;
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

    private void OnMousePostion(InputAction.CallbackContext context)
    {
        //Vector2 pos = Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue());
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 Worldpos = Camera.main.ScreenToWorldPoint(mousePos);
        Worldpos -= transform.position;
       
        anim.SetFloat(InputX_String, Worldpos.x);
        anim.SetFloat(InputY_String, Worldpos.y);
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

           
            anim.SetBool(Input_Move, true);
        }
        if(context.canceled)
        {
            Inputdir = context.ReadValue<Vector2>();
      
            anim.SetBool(Input_Move, false);
        }

        
    }

    private void Update()
    {
        transform.Translate(Time.deltaTime*speed*Inputdir);
    }


}
