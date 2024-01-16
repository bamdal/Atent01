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
    Vector3 Inputdir = Vector3.zero;
    Vector3 InputMouse = Vector3.zero;
    Vector3 Worldpos;
    bool isAttack = true;
    public float speed = 6.0f;
    Animator anim;
    readonly int InputX_String = Animator.StringToHash("DirectionX");
    readonly int InputY_String = Animator.StringToHash("DirectionY");
    readonly int Input_Move = Animator.StringToHash("IsMoving");
    Transform AttackPosition;
    public float maxHp = 50.0f;
    float hp;
    float angle;
    public float Hp
    {
        get => hp;
        private set
        {
            if (hp != value)
            {

            }
            hp = value;
        }
    }

    private void Awake()
    {
        inputSystems = new InputSystems();
        anim = GetComponent<Animator>();
        AttackPosition = transform.GetChild(1);
        
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
        Worldpos = Camera.main.ScreenToWorldPoint(mousePos);
        Worldpos -= transform.position;
        Worldpos.z = 0;

        
        anim.SetFloat(InputX_String, Worldpos.x);
        anim.SetFloat(InputY_String, Worldpos.y);
    }

    private void OnRClick(InputAction.CallbackContext context)
    {

    }

    private void OnLClick(InputAction.CallbackContext context)
    {
        if (context.performed && isAttack)
        {
            StartCoroutine(StartAttack());
            
        }
        if(context.canceled) 
        {
            

        }

    }

    IEnumerator StartAttack()
    {
        isAttack = false;
        AttackPosition.position = Worldpos.normalized + transform.position;
        angle = Mathf.Atan2(Worldpos.normalized.y, Worldpos.normalized.x) * Mathf.Rad2Deg;
        AttackPosition.rotation = Quaternion.Euler(0, 0, angle - 90);
        AttackPosition.gameObject.SetActive(true);
        yield return new WaitForSeconds(AttackPosition.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        AttackPosition.gameObject.SetActive(false);
        isAttack = true;
    }


    private void OnDash(InputAction.CallbackContext context)
    {

    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Inputdir = context.ReadValue<Vector2>();


            anim.SetBool(Input_Move, true);
        }
        if (context.canceled)
        {
            Inputdir = context.ReadValue<Vector2>();

            anim.SetBool(Input_Move, false);
        }


    }

    private void Update()
    {
        transform.Translate(Time.deltaTime * speed * Inputdir);
    }


}
