using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]

public class Player : MonoBehaviour
{

    InputSystems inputSystems;
    Vector3 Inputdir = Vector3.zero;
    Rigidbody2D rigid2d;

    Vector3 Worldpos;
    bool isAttack = true;
    float originalspeed;
    public float speed = 6.0f;
    public float dashSpeed = 4.0f;

    Animator anim;
    readonly int InputX_String = Animator.StringToHash("DirectionX");
    readonly int InputY_String = Animator.StringToHash("DirectionY");
    readonly int Input_Move = Animator.StringToHash("IsMoving");
    Transform AttackPosition;
    public float maxHp = 5.0f;
    public float maxSt = 10.0f;
    public Slider HPslider;
    public Slider STslider;
    float hp;
    float st;
    float angle;
    Coroutine stcoroutine;
    bool Isdash = true;
    public float dashSt = 0.1f;
    public float regenSt = 0.5f;

   
    public float Hp
    {
        get => hp;
        private set
        {
            hp = value;
            hp = Mathf.Clamp(value, 0, maxHp);
            if (hp < 0.1f)
            {
               
                OnDie();
            }
           
            
     
        }
    }

    public float St 
    {
        get => st;
        set 
        {
            st = value;
            st = Mathf.Clamp(value, 0, maxSt);
        } 
    }

    int score = 0;
    public Action<int> onScoreChange;
    public int Score
    {
        get => score; // 읽기는 public
        private set // 쓰기는 private
        {
            if (score != value)
            {

                score = Mathf.Min(value, 99999);
               
                onScoreChange?.Invoke(score);
            }

        }
    }
    public void AddScore(int getScore)
    {
        Score += getScore;

    }

    private void Awake()
    {
        inputSystems = new InputSystems();
        anim = GetComponent<Animator>();
        rigid2d = GetComponent<Rigidbody2D>();
        AttackPosition = transform.GetChild(0);
        Hp = maxHp;
        St = maxSt;
        HPslider.maxValue = maxHp;
        HPslider.value = Hp;
        STslider.maxValue = maxSt;
        STslider.value = St;
        originalspeed = speed;
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
        if(context.performed) 
        {
            Isdash = true;
            speed += dashSpeed;
            stcoroutine = StartCoroutine(Stamina());
        }
        if(context.canceled)
        {
            Isdash = false;
            speed = originalspeed;
        }
    }

    IEnumerator Stamina()
    {
        while(Isdash) 
        {
            yield return new WaitForSeconds(0.01f);
            St -= dashSt ; 
            STslider.value = St;
            if(St < 0.1f)
                speed = originalspeed;

        }

        while (St != maxSt && !Isdash)
        {
            yield return new WaitForSeconds(0.01f);
            St += regenSt;
            STslider.value = St;


        }
 

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



    private void FixedUpdate()
    {
        rigid2d.MovePosition(rigid2d.position + (Vector2)(Time.fixedDeltaTime * speed * Inputdir));

    }

    public void PlayerHit(float damage)
    { 
        if(!anim.GetBool("IsHit"))
            StartCoroutine(PlayerEmun(damage));
    }

    IEnumerator PlayerEmun(float damage)
    {
        anim.SetBool("IsHit", true);
        Hp -= damage;
        HPslider.value = Hp;
    
        yield return new WaitForSeconds(0.7f);
        anim.SetBool("IsHit", false);
    }

    private void OnDie()
    {
        //SceneManager.LoadScene("");
   
        SceneManager.LoadScene("GameOverScene");

    }
}
