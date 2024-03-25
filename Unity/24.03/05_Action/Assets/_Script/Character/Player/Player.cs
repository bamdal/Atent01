using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Player : MonoBehaviour, IHealth, IMana
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

    [Range(0, attackAnimationLength)]
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

    /// <summary>
    /// 무기 이펙트 변화를 알리는 델리게이트
    /// </summary>
    Action<bool> onWeaponEffectEnable;

    /// <summary>
    /// 아이템을 줏을 수 있는 거리 (아이템을 버릴 수 있는 최대 거리)
    /// </summary>
    public float ItemPickupRange = 2.0f;

    /// <summary>
    /// 플레이어가 가진 인벤토리
    /// </summary>
    Inventory inven;

    public Inventory Inventory => inven;

    /// <summary>
    /// 현재 돈
    /// </summary>
    int money = 0;

    /// <summary>
    /// 돈의 변경을 알리는 델리게이트
    /// </summary>
    public Action<int> onMoneyChange;

    public int Money
    {
        get => money;
        set
        {
            if (money != value)
            {
                money = value;
                onMoneyChange?.Invoke(money);
            }
        }
    }
    /// <summary>
    /// 플레이어의 현재 HP
    /// </summary>
    float hp = 100.0f;


    public float HP
    {
        get => hp;
        set
        {
            if (IsAlive)    // 플레이어가 살아있을 때만 HP변화
            {
                hp = value;
                if (hp <= 0)    // HP 0이하 사망
                {
                    Die();
                }
                hp = Mathf.Clamp(hp, 0, MaxHP);     // 최소 최대 사이로 숫자 유지
                onHealthChange?.Invoke(hp/MaxHP);   // 델리게이트로 HP변화 알림
                Debug.Log($"{this.gameObject.name} HP : {hp}");
            }
        }
    }

    /// <summary>
    /// 플레이어의 최대 HP 
    /// </summary>
    float maxHP = 100.0f;

    public float MaxHP => maxHP;

    /// <summary>
    /// HP의 변경을 알리는 델리게이트
    /// </summary>
    public Action<float> onHealthChange { get; set; }

    /// <summary>
    /// 플레이어의 생존 여부를 확인하기 위한 프로퍼티
    /// </summary>
    public bool IsAlive => HP > 0;

    /// <summary>
    /// 플레이어의 사망을 알리는 델리게이트
    /// </summary>
    public Action onDie { get; set; }

    /// <summary>
    /// 플레이어의 현재 마나
    /// </summary>
    float mp = 150.0f;

    public float MP 
    {
        get => mp;
        set
        { 
            if (IsAlive)
            {
                mp = value;
                mp = Mathf.Clamp(mp, 0, MaxMP);
                onManaChange?.Invoke(mp/MaxMP);
                Debug.Log($"{this.gameObject.name} Mana {mp}");
            }

        }
    }          

    /// <summary>
    /// 플레이어의 최대 마나
    /// </summary>
    float maxMP = 150.0f;

    public float MaxMP => maxMP;

    /// <summary>
    /// 마나가 변경되었음을 알리는 델리게이트(현재/최대)
    /// </summary>
    public Action<float> onManaChange { get; set; }

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
        inputController.onItemPickup += OnItemPickInput;

    }

    private void Start()
    {
        inven = new Inventory(this);    // itemDataManager가 게임매니저에 있어서 반드시 start에 있어야함
        if(GameManager.Instance.InventoryUI != null)
        {
            GameManager.Instance.InventoryUI.InitializeInventory(Inventory);    // 인벤토리와 내 UI를 연결
        }

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

    /// <summary>
    /// 주변에 있는 아이템을 습득하는 함수
    /// </summary>
    private void OnItemPickInput()
    {
        // 주변에 있는 Item 레이어의 콜라이더 전부 찾기
        Collider[] itemColliders = Physics.OverlapSphere(transform.position, ItemPickupRange, LayerMask.GetMask("Item"));
        foreach (Collider collider in itemColliders)
        {
            ItemObject item = collider.GetComponent<ItemObject>();

            IConsumable consumItem = item.ItemData as IConsumable;
            if (consumItem == null)
            {
                // 일반 아이템
                if (Inventory.AddItem(item.ItemData.code))  // 인벤토리 추가 시도
                {
                    item.End();                      // 아이템 제거
                }

            }
            else
            {
                consumItem.Consume(gameObject);
                item.End();
            }
        }
    }

    public void Die()
    {
        onDie?.Invoke();
        Debug.Log("플레이어 사망");
    }

    /// <summary>
    /// 플레이어의 체력을 지속적으로 증가시켜 주는 함수, 초당 totalRegen/duration 만큼 회복
    /// </summary>
    /// <param name="totalRegen">전체 회복량</param>
    /// <param name="duration">전체 회복시간</param>
    public void HealthRegenerate(float totalRegen, float duration)
    {
        StartCoroutine(RegenCoroutine(totalRegen, duration,true));
    }





    /// <summary>
    /// 플레이어의 체력을 틱단위로 회복시켜주는 함수,
    /// 전체 회복량 = tickRegen * totalTickCount, 전체 회복시간 = tickInterval * totalTickCount
    /// </summary>
    /// <param name="tickRegen">틱 당 회복량</param>
    /// <param name="tickInterval">틱 간의 시간 간격</param>
    /// <param name="totalTickCount">전체 틱 수</param>
    public void HealthRegenerateByTick(float tickRegen, float tickInterval, uint totalTickCount)
    {
        StartCoroutine(RegenByTickCoroutine(tickRegen, tickInterval, totalTickCount,true));
    }

    /// <summary>
    /// 플레이어의 마나를 지속적으로 증가시켜 주는 함수, 초당 totalRegen/duration 만큼 회복
    /// </summary>
    /// <param name="totalRegen">전체 회복량</param>
    /// <param name="duration">전체 회복시간</param>
    public void ManaRegenerate(float totalRegen, float duration)
    {
        StartCoroutine(RegenCoroutine(totalRegen, duration,false));
    }


    /// <summary>
    /// 플레이어의 마나를 틱단위로 회복시켜주는 함수,
    /// 전체 회복량 = tickRegen * totalTickCount, 전체 회복시간 = tickInterval * totalTickCount
    /// </summary>
    /// <param name="tickRegen">틱 당 회복량</param>
    /// <param name="tickInterval">틱 간의 시간 간격</param>
    /// <param name="totalTickCount">전체 틱 수</param>
    public void ManaRegenerateByTick(float tickRegen, float tickInterval, uint totalTickCount)
    {
        StartCoroutine(RegenByTickCoroutine(tickRegen, tickInterval, totalTickCount,false));
    }

    /// <summary>
    /// 플레이어의 체력,마나를 지속적으로 증가시켜 주는 코루틴, 초당 totalRegen/duration 만큼 회복
    /// </summary>
    /// <param name="totalRegen">전체 회복량</param>
    /// <param name="duration">전체 회복시간</param>
    /// <param name="isHP">true면 체력, false면 마나</param>
    /// <returns></returns>
    IEnumerator RegenCoroutine(float totalRegen, float duration, bool isHP)
    {
        float regenPerSec = totalRegen / duration;
        float timeElapsed = 0.0f;
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            if (isHP)
            {
                HP += regenPerSec * Time.deltaTime;

            }
            else
            {
                MP += regenPerSec * Time.deltaTime;

            }
            yield return null;
        }
    }

    /// <summary>
    /// 플레이어의 마나를 틱단위로 회복시켜주는 함수,
    /// 전체 회복량 = tickRegen * totalTickCount, 전체 회복시간 = tickInterval * totalTickCount
    /// </summary>
    /// <param name="tickRegen">틱 당 회복량</param>
    /// <param name="tickInterval">틱 간의 시간 간격</param>
    /// <param name="totalTickCount">전체 틱 수</param>
    /// <param name="isHP">true면 체력, false면 마나</param>
    /// <returns></returns>
    IEnumerator RegenByTickCoroutine(float tickRegen, float tickInterval, uint totalTickCount, bool isHP)
    {
        WaitForSeconds wait = new WaitForSeconds(tickInterval);
        uint count = totalTickCount;
        while (count > 0)  // totalTickCout 회수 만큼 회복
        {
            count--;
            if (isHP)
            {
                HP += tickRegen;    // tickRegen 만큼 증가

            }
            else
            {
                MP += tickRegen;    // tickRegen 만큼 증가

            }
            yield return wait;  // tickInterval 만큼 대기
        }

    }




#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, Vector3.up, ItemPickupRange,2.0f);
    }



#endif
}

