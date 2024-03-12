using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    /// <summary>
    /// 걷는 속도
    /// </summary>
    public float walkSpeed = 3.0f;

    /// <summary>
    /// 달리는 속도
    /// </summary>
    public float runSpeed = 5.0f;

    /// <summary>
    /// 현재 속도
    /// </summary>
    float currentSpeed = 0.0f;

    /// <summary>
    /// 남아있는 쿨타임
    /// </summary>
    float coolTime = 0.00f;

    [Range(0,attackAnimationLength)]
    /// <summary>
    /// 쿨타임 초기화용 함수
    /// </summary>
    public float maxCoolTime = 0.3f;

    /// <summary>
    /// 공격애니메이션 재생시간
    /// </summary>
    const float attackAnimationLength = 0.533f;
   

    /// <summary>
    /// 이동 모드
    /// </summary>
    enum MoveMode
    {
        Walk = 0,   // 걷기 모드
        Run         // 달리기 모드
    }

    /// <summary>
    /// 현재 이동모드
    /// </summary>
    MoveMode currentMoveMode = MoveMode.Run;

    MoveMode CurrnetMoveMode
    {
        get => currentMoveMode;
        set
        {
            currentMoveMode = value;    // 상태 변경
            if (currentSpeed > 0.0f)    // 이동 중인지 아닌지 확인
            {
                // 이동 중이면 모드에 맞게 속도와 애니메이션 변경
                MoveSpeeChange(currentMoveMode);
            }

        }
    }

    /// <summary>
    /// 무기 장비할 트랜스폼
    /// </summary>
    GameObject weaponParent;

    /// <summary>
    /// 방패 장비할 트랜스폼
    /// </summary>
    GameObject shieldParent;

    PlayerInputController inputController;

    /// <summary>
    /// 입력된 이동 방향
    /// </summary>
    Vector3 inputDirection = Vector3.zero;  // y는 무조건 바닥 높이

    /// <summary>
    /// 캐릭터의 목표방향으로 회전시키는 회전
    /// </summary>
    Quaternion targetRotation = Quaternion.identity;

    /// <summary>
    /// 캐릭터 회전 속도
    /// </summary>
    public float turnSpeed = 10.0f;

    // 컴포넌트들
    Animator animator;
    CharacterController characterController;

    // 애니메이터용 해시값 및 상수
    readonly int Speed_Hash = Animator.StringToHash("Speed");
    const float AnimatorStopSpeed = 0.0f;
    const float AnimatorWalkSpeed = 0.3f;
    const float AnimatorRunSpeed = 1.0f;
    readonly int Attack_Hash = Animator.StringToHash("Attack");


    Action<bool> onWeaponEffectEnable;

    private void Awake()
    {
        weaponParent = GameObject.Find("weapon_r");
        shieldParent = GameObject.Find("weapon_l"); 


        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        inputController = GetComponent<PlayerInputController>();
        inputController.onMove += OnMoveInput;
        inputController.onMoveModeChange += OnMoveModeChangeInput;
        inputController.onAttack += OnAttackInput;


    }

    private void Start()
    {
  

        Weapon weapon = weaponParent.GetComponentInChildren<Weapon>();
        onWeaponEffectEnable = weapon.EffectEnable;
        
    }

    private void Update()
    {
        characterController.Move(Time.deltaTime * currentSpeed * inputDirection);   // 좀 더 수동
        //characterController.SimpleMove(currentSpeed *  inputDirection);           // 좀 더 자동

        // 목표 회전으로 변경
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

        coolTime -= Time.deltaTime;
    }

    /// <summary>
    /// 이동 입력에 대해 델리게이트로 받는 함수
    /// </summary>
    /// <param name="input">입력된 이동 방향</param>
    /// <param name="move">달리고 있는지 판단하는 함수(true면 키 입력중)</param>
    private void OnMoveInput(Vector2 input, bool move)
    {

        inputDirection.x = input.x;     // 입력 방향 저장
        inputDirection.y = 0;
        inputDirection.z = input.y;

        if (move)
        {
            // 카메라의 y 회전만 따로 추출
            Quaternion camY = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
            inputDirection = camY * inputDirection; // 입력방향을 카메라의 y회전과 같은 정도로 회전 시키기
            targetRotation = Quaternion.LookRotation(inputDirection);   // 목표회전 저장

            // 이동 모드 변경
            MoveSpeeChange(CurrnetMoveMode);
        }
        else
        {
            // 입력을 끝낸 상황
            currentSpeed = 0.0f;    // 정지 시키기
            animator.SetFloat(Speed_Hash, AnimatorStopSpeed);
        }
    }

    /// <summary>
    /// 이동 변화를 입력하는 함수
    /// </summary>
    private void OnMoveModeChangeInput()
    {
        if (CurrnetMoveMode == MoveMode.Walk)
        {
            CurrnetMoveMode = MoveMode.Run;
        }
        else
        {
            CurrnetMoveMode = MoveMode.Walk;
        }
    }

    /// <summary>
    /// 공격입력에 대해 델리게이트로 실행되는함수
    /// </summary>
    private void OnAttackInput()
    {
        // 쿨타임이 다 되었고,
        // 서있거나 걷는 상태일때 공격가능
        if (coolTime < 0.0f && (currentSpeed < 0.001f || CurrnetMoveMode == MoveMode.Walk))
        {
            animator.SetTrigger(Attack_Hash);
            coolTime = maxCoolTime;
        }

        

    }


    /// <summary>
    /// 모드에 따라 이동속도를 변경하는 함수
    /// </summary>
    /// <param name="mode">현재 모드</param>
    private void MoveSpeeChange(MoveMode mode)
    {
        // 눌려지고 있는 상황
        switch (mode)    // 이동 모드에 따라 속도와 애니메이션 변경
        {
            case MoveMode.Walk:
                currentSpeed = walkSpeed;
                animator.SetFloat(Speed_Hash, AnimatorWalkSpeed);
                break;
            case MoveMode.Run:
                currentSpeed = runSpeed;
                animator.SetFloat(Speed_Hash, AnimatorRunSpeed);
                break;
        }
    }


    /// <summary>
    /// 무기와 방패를 보여줄지 말지를 결정하는 함수
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowWeaponAndShield(bool isShow = true)
    {
        weaponParent.transform.GetChild(0).gameObject.SetActive(isShow);
        shieldParent.transform.GetChild(0).gameObject.SetActive(isShow);
    }

    /// <summary>
    /// 무기 이펙트를 키거나 끄라는 신호를 보내는 함수
    /// </summary>
    /// <param name="isShow">true일 때 보이고, false일때 안보인다</param>
    public void ShowWeaponEffect(bool isShow = true)
    {
        onWeaponEffectEnable?.Invoke(isShow);
    }
}

