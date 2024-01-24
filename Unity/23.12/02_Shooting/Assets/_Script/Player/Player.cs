using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(Animator))] // 반드시 특정 컴포넌트가 필요한 경우 추가 없으면 강제로 추가함

public class Player : MonoBehaviour
{
    // InputManager : 기존의 유니티 입력 방식
    // 장점 : 간단하다.
    // 단점 : 인풋렉, 계속 눌렸는지 확인하는데 전기낭비가 심함(Busy-wait이 발생할 수 밖에 없다)

    // InputSystem : 유니티의 새로운 입력방식
    // Event-driven 방식 적용

    PlayerInputActions inputActions;
    Vector3 inputDir = Vector3.zero;
    Vector3 shoot = Vector3.zero;
    Animator anim;
    readonly int InputY_String = Animator.StringToHash("InputY"); // InputY를 해시값으로 바꿔서 애니메이션파라미터 설정에 사용할수있다.
    Rigidbody2D rigid2d;
    SpriteRenderer spriteRenderer;


    /// <summary>
    /// public 맴버변수는 인스펙터 창에서 확인 가능
    /// </summary>
    //[Range(0.0f,0.5f)] // 스크롤바로 값 조절가능
    public float moveSpeed = 5.0f; // 플레이어 이동속도
    public GameObject bulletPrefeb;

    Transform[] fireTransforms;// 총알 발사 위치

    GameObject fireFlash; // 총알 발사 이펙트

    WaitForSeconds flashWait; // 플래시 대기시간

    IEnumerator fireCoroution;

    public float fireInterval = 0.5f;


    /// <summary>
    /// 파워 3단계 에서 파워업 아이템을 먹었을때 얻는 보너스 점수
    /// </summary>
    public int powerBonus = 1000;

    /// <summary>
    /// 최대 파워단계
    /// </summary>
    private const int maxPower = 3;
    /// <summary>
    /// 최소 파워단계
    /// </summary>
    private const int minPower = 1;

    /// <summary>
    /// 발사 각도
    /// </summary>
    private const float fireAngle = 30.0f;
    /// <summary>
    /// 현재 파워
    /// </summary>
    private int power = 1;
    private int Power
    {
        get => power;
        set
        {
            if (power != value) // 변경이 있을때 처리
            {
                power = value;
                if (power > maxPower)
                {
                    AddScore(powerBonus); // 파워가 최대치를 벗어나면 보너스 점수
                }
                power = Mathf.Clamp(power, minPower, maxPower); // 파워는 최소~최대 단계만 존재

                RefreshFirePosition(); // 총알 발사 위치 조정
            }
        }
    }

    /// <summary>
    /// 플레이어의 남은 생명
    /// </summary>
    private int life = 3;

    /// <summary>
    /// 플레이어 시작시 생명
    /// </summary>
    const int startLife = 3;


    /// <summary>
    /// 생명을 설정하고 확인하기 위한 프로퍼티
    /// </summary>
    private int Life
    {
        get => life;
        set
        {
            if (life != value) // 변화가 있을 때만 처리
            {
                if (IsAlive&&value<life)
                {
                    OnHit(); // 맞고나서 살아있으면 맞은처리
                }
                life = value;
                life = Mathf.Clamp(life, 0, startLife);
                if (!IsAlive)
                {
                    OnDie(); // 맞고나서 죽었으면 죽은처리
                }



                onLifeChange?.Invoke(life); // 생명변화가 있었음을 알림
            }


        }
    }
    /// <summary>
    /// 살았는지 죽었는지 확인하기 위한 프로퍼티
    /// </summary>
    private bool IsAlive => life > 0;

    /// <summary>
    /// 생명변화가 있음을 알리는 델리게이트
    /// </summary>
    public Action<int> onLifeChange;

    /// <summary>
    /// 맞았을때 무적시간
    /// </summary>
    public float invincibleTime = 2.0f;

    /// <summary>
    /// 죽었음을 알리는 델리게이트(점수도 같이 전달)
    /// </summary>
    public Action<int> onDie;

    /// <summary>
    /// 점수가 변경되었을때 알리는 델리게이트 (파라메터: 변경된 점수)
    /// </summary>
    public Action<int> onScoreChange;


    //[SerializeField] // public이 아닌 경우에도 인스펙터 창에서 확인이 가능


    // 이 스크립트가 포함된 게임오브젝트가 생성완료되면 호출
    private void Awake()
    {
        inputActions = new PlayerInputActions();
        anim = GetComponent<Animator>(); // 이 스크립트가 들어있는 게임 오브젝트에서 컴포넌트를 가져온다.
                                         // 단 GetComponent는 매우느리다.
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid2d = GetComponent<Rigidbody2D>();
        // 게임 오브젝트 찾는 방법
        // GameObject.Find("FirePosition"); // 이름으로 게임 오브젝트 찾기 중복된건 먼저 발견한걸 불러옴
        // GameObject.FindAnyObjectByType<Transform>(); // 특정 컴포넌트를 가지고 있는 오브젝트 찾기 뭐가 나올지 모름
        // GameObject.FindFirstObjectByType<Transform>(); // 특정 컴포넌트를 가지고 있는 첫번째 오브젝트 찾기
        // GameObject.FindGameObjectWithTag("Player"); // 게임 오브젝트의 태그를 기준으로 찾는 함수
        // GameObject.FindGameObjectsWithTag("Player"); // 특정 태그를 가진 모든 오브젝트들을 배열에 담음
        Transform fireRoot = transform.GetChild(0);
        fireTransforms = new Transform[fireRoot.childCount];
        for (int i = 0; i < fireTransforms.Length; i++)
        {
            fireTransforms[i] = fireRoot.GetChild(i);
        }
        fireFlash = transform.GetChild(1).gameObject; // 두번째 자식 오브젝트 찾기
        // transform.childCount; // 이 게임오브젝트의 자식숫자
        flashWait = new WaitForSeconds(0.1f);
        fireCoroution = FireCoroution();

    }



    // 이 스크립트가 포함된 게임오브젝트가 활성화 되면 호출
    private void OnEnable()
    {
        inputActions.Player.Enable();                   // 활성화될 때 Player액션맵을 활성화
        inputActions.Player.Fire.performed += OnFireStart;   // Player액션맵의 Fire 액션에 OnFire함수를 연결(눌렀을 때만 연결된 함수실행)
        inputActions.Player.Fire.canceled += OnFireEnd;   // Player액션맵의 Fire 액션에 OnFire함수를 연결(땠을 떄만 연겨된 함수실행)
        inputActions.Player.Boost.performed += OnBoost;
        inputActions.Player.Boost.canceled += OnBoost;
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
    }




    // 이 스크립트가 포함된 게임오브젝트가 비활성화 되면 호출
    private void OnDisable()
    {
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Boost.canceled -= OnBoost;
        inputActions.Player.Boost.performed -= OnBoost;
        inputActions.Player.Fire.canceled -= OnFireEnd;   // Player액션맵의 Fire 액션에 OnFire함수를 연결해제(땠을 떄만 연겨된 함수실행)
        inputActions.Player.Fire.performed -= OnFireStart;   // Player액션맵의 Fire 액션에 OnFire함수를 연결해제
        inputActions.Player.Disable();                  // Player액션맵을 비활성화
    }



    /// <summary>
    /// Fire액션이 발동했을때 실행시킬 함수
    /// </summary>
    /// <param name="context">입력 관련 정보가 들어있는 구조체 변수</param>
    private void OnFireStart(InputAction.CallbackContext _)
    {


        /*            Debug.Log("OnFire : 눌러짐");
                    //Instantiate(bulletPrefeb, transform); 총알이 자식으로 들어감

                    Fire(fireTransform.position);
                    StartCoroutine(FlashEffect());*/
        StartCoroutine(fireCoroution);


    }
    private void OnFireEnd(InputAction.CallbackContext _)
    {
        StopCoroutine(fireCoroution); // 연사중지
    }

    IEnumerator FireCoroution()
    {

        while (true)
        {
            /*            switch(PowerUp)
                        {
                            case 0:
                                Fire(fireTransform.position);
                                break; 
                            case 1:
                                Fire(fireTransform.position,bulletangle);
                                Fire(fireTransform.position,-bulletangle);
                                break;
                            default:
                                Fire(fireTransform.position, bulletangle);
                                Fire(fireTransform.position, -bulletangle);
                                Fire(fireTransform.position);
                             break;

                        }*/
            for (int i = 0; i < Power; i++)
            {
                Fire(fireTransforms[i]);
            }
            StartCoroutine(FlashEffect());
            yield return new WaitForSeconds(fireInterval);
        }
    }

    /// <summary>
    /// 총알을 하나 발사하는 함수
    /// </summary>
    /// <param name="position">총알이 발사될 위치</param>
    /// <param name="angle">총알이 발사될 각도(디폴트로 0)</param>
    void Fire(Transform fireTransform) //파라메터에 값을 넣어두면 함수호출때 않쓰면 디폴트값을 사용
    {                                               // 디폴트 파라메터는 마지막에 작성해야 효율적
        //Instantiate(bulletPrefeb, position, Quaternion.identity);
        Factory.Instance.GetBullet(fireTransform.position, fireTransform.eulerAngles.z); // 팩토리를 이용해 총알 생성

    }

    IEnumerator FlashEffect()
    {


        fireFlash.SetActive(true);// 플래시 이펙트
        yield return flashWait;
        fireFlash.SetActive(false);


    }

    private void OnBoost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
        if (context.canceled)
        {

        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // scope : 변수나 함수의 사용 가능한 범위
        inputDir = context.ReadValue<Vector2>();
        //Debug.Log($"OnMove : {inputDir}");

        //this.transform.position = new Vector3(1, 0, 0); // 현재 이 오브젝트를 (1, 0, 0)에 보내라
        //this.transform.position += new Vector3(1, 0, 0); // 현재 위치에서 (1, 0, 0)만큼 움직여라
        //this.transform.position += Vector3.right;
        //transform.position += (Vector3)dir;
        anim.SetFloat(InputY_String, inputDir.y);
    }

    // 실습
    // 누르고 있으면 계속 그쪽 방향으로 이동하게 만들기


    // 이 스크립트가 포함된 게임오브젝트의 첫번째 Update함수가 실행되기 직전에 실행
    private void Start()
    {
        power = 1;
        Life = startLife;

    }

    private void Update()
    {
        //인풋 매니져 방식
        /*        if (Input.GetKeyDown(KeyCode.A))
                {
                    Debug.Log("A 입력");
                }

                if (Input.GetKeyUp(KeyCode.A))
                {
                    Debug.Log("A 입력때기");
                }*/

        /*        bool hasControl = (inputDir != Vector3.zero); // 벡터는 float형이기에 if를 사용하는것이 
                if (hasControl)                                 // 자원낭비고 그냥 쓰는것보다 더 비효율적임
                { 
                    this.transform.position += inputDir * 0.1f; ;
                }*/

        // Time.deltaTime : 프레임간의 시간 간격(가변적)
        //transform.Translate(Time.deltaTime * moveSpeed * inputDir); // 기능은 this.transform.position += inputDir; 같다
        // 1초당 moveSpeed속도로 inputDir 방향으로 이동

    }

    // 고정된 시간간격으로 호출되는 업데이트(물리연산 처리용 업데이트) 
    private void FixedUpdate()
    {
        if (IsAlive) // 살아있을때만 이동
        {
            //transform.Translate(Time.deltaTime * moveSpeed * inputDir);
            rigid2d.MovePosition(rigid2d.position + (Vector2)(Time.fixedDeltaTime * moveSpeed * inputDir));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*        // 충돌이 시작했을 때 실행
                Debug.Log($"OnCollisionEnter2D : {collision.gameObject.name}");
                //Destroy(collision.gameObject); // 충돌 대상제거
                collision.gameObject.SetActive(false);
        */

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Life--;
        }
        else if (collision.gameObject.CompareTag("PowerUp"))
        {
            Power++;
            Life++;
            collision.gameObject.SetActive(false);
        }


    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 충돌 중인 상태에서 움직일 때 실행
        //Debug.Log("OnCollisionStay2D");
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // 충돌 상태에서 떨어졌을 때 실행

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 겹치기 시작했을 때 실행
        /*        if (collision.CompareTag("PowerUp"))
                {
                    PowerUp++;
                    Debug.Log("OnTriggerStay2D");
                }
                if(PowerUp > 2) 
                {
                    AddScore(1000);
                }*/
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 겹쳐있는 상태에서 움직일 때 실행
        //Debug.Log("OnTriggerStay2D");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 겹쳐있던 상태에서 떨어졌을때 실행

    }

    /// <summary>
    /// 점수추가 함수
    /// </summary>
    /// <param name="getScore">새로 얻은 점수</param>
    public void AddScore(int getScore)
    {
        Score += getScore;

    }

    /// <summary>
    /// 파워단계에 따라 총알 발사 위치 조정
    /// </summary>
    private void RefreshFirePosition()
    {
        for (int i = 0; i < maxPower; i++)
        {
            if (i < Power) // 파워 단계에 맞게 보이는 부분 조정
            {
                float startAngle = (power - 1) * (fireAngle * 0.5f);
                float angleDelta = i * -fireAngle;
                fireTransforms[i].rotation = Quaternion.Euler(0, 0, startAngle + angleDelta);

                fireTransforms[i].localPosition = Vector3.zero; // 초기화
                fireTransforms[i].Translate(0.5f, 0.0f, 0.0f); // 로컬기준 오른쪽으로 이동

                fireTransforms[i].gameObject.SetActive(true); // 활성화
            }
            else
            {
                fireTransforms[i].gameObject.SetActive(false); // 비활성화
            }
        }
    }


    /// <summary>
    /// 맞았을 때 ㅣㄹ행되는 함수
    /// </summary>
    private void OnHit()
    {
        //Debug.Log($"플레이어 생명이 {Life}남았다");

        // 파워 단계 떨어트리기
        Power--;
        StartCoroutine(InvincibleMode()); // 무적모드에 들어감
    }
    /// <summary>
    /// 무적 모드 처리용 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator InvincibleMode()
    {
        // 일시적으로 무적(무적일 때 화면 밖으로 나가면 안된다, 적과 충돌하면 안된다. 2초간) 
        // - 무적일 때 화면 밖으로 나가면 안된다
        // - 적과 충돌하면 안된다.
        // - 무적 상태 표시 (플레이어의 비행기가 깜박인다)

        //Physics2D.IgnoreLayerCollision(6,7);
        gameObject.layer = LayerMask.NameToLayer("Invincible"); // 레이어를 무적레이어로 변경
        float timeElapsed = 0.0f;
        while (timeElapsed < invincibleTime) // invincibleTime동안 계속하기
        {
            timeElapsed += Time.deltaTime;
            float alpha = (Mathf.Cos(timeElapsed * 30.0f) + 1.0f) * 0.5f; // 코사인 결과를 1~0사이로 변경
            spriteRenderer.color = new Color(1, 1, 1, alpha);           // 알파에 지정(깜박거리기)
            yield return null;

        }
        gameObject.layer = LayerMask.NameToLayer("Player"); // 레이어를 다시 플레이어로 되돌리기
        spriteRenderer.color = Color.white;                 // 알파값도 원상복구

        //Physics2D.IgnoreLayerCollision(6,7,false);
    }

    private void OnDie()
    {
        Debug.Log($"플레이어가 죽었다");

        // 충돌이 안 일어나야함
        Collider2D body = GetComponent<Collider2D>();
        body.enabled = false;

        // 내가 터지는 이팩트
        Factory.Instance.GetExplosionEffect(transform.position);

        // 입력도 안되어야 함
        inputActions.Player.Disable();

        // 이동 초기화, 총알 발사 코루틴 정지 => disable하면서 자동으로 입력이 디폴트가됨

        // 튕겨나가는 듯한 연출을 보여줘야 한다.
        rigid2d.gravityScale = 1.0f;
        rigid2d.freezeRotation = false;
        rigid2d.AddTorque(1000);
        rigid2d.AddForce(Vector2.left * 10.0f, ForceMode2D.Impulse);// Force 지속적인힘, Impulse 순간적인힘
        
        // 사망했음을 알림
        onDie?.Invoke(score);    
    }

    public void Test_Die()
    {
        Life = 0;
    }
    int score = 0;
    public int Score
    {
        get => score; // 읽기는 public
        private set // 쓰기는 private
        {
            if (score != value)
            {

                score = Mathf.Min(value, 99999);
                onScoreChange?.Invoke(score); // 이 델리게이트에 함수를 등록한 모든 대상에게 변경된 점수를 알림
            }

        }
    }


# if UNITY_EDITOR // Unity 에디터에서만 실행됨
    public void Test_PowerUp()
    {
        Power++;
    }
    public void Test_PowerDown()
    {
        Power--;
    }

    public void Test_SetScore(int score)
    {
        Score += score;
    }
#endif
}
