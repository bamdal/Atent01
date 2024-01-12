using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyBase : RecycleObject
{
    // 3. 적은 위아래로 파도 치듯이 움직인다.
    // 4. 계속 왼쪽 방향으로 이동한다.
    /*    float MaxY = 4.0f;
        float MinY = -4.0f;
        float VectorY=0;
        float WorldY;
        Vector3 EnemyVector = Vector3.left;
        public float EnemySpeedX = 1.0f;
        float EnemySpeedY = 1.0f;
    */
    public float speed = 1.0f;
/*    public float amplitude = 3.0f; //높이제한

    public float frequency = 2.0f; // 사인 그래프가 황복하는데 걸리는 시간 증폭용
*/

    float maxHP =3.0f;
    float hp = 3.0f;

    /// <summary>
    /// 적을 해치웠을때 얻을 점수
    /// </summary>
    public int score= 10;

    /// <summary>
    /// 적이 죽을때 실행될 델리게이트
    /// </summary>
    Action onDie;
 // 람다식, 람다함수(Lambda), 익명함수



    public float Hp 
    { 
        get => hp;
        private set
        {
            if (hp != value)
/*            {
                Debug.Log("맞음");
            }*/

            hp = value;
            hp = Math.Clamp(value, 0.0f, MaxHP);
            if(hp ==0.0f) 
            {
               
                OnDie();
            }
        }
        
    }

    public float MaxHP { get => maxHP; set => maxHP = value; }

    Player player;




    private void Awake()
    {
/*        VectorY = transform.position.y;
        WorldY = VectorY;
        Mathf.Cos(Time.deltaTime*90.0f * Mathf.Deg2Rad); //90도를 라디안으로 변경해 Cos결과구하기*/
    }



    protected override void OnEnable()
    {
        base.OnEnable();
        OnInitialize();
/*        spawnY = transform.position.y;
        elapsedTime = 0.0f;*/

        //Action aaa = () => Debug.Log("람다함수"); // 파라메터 없는 람다식
        //Action<int> bbb = (x) => Debug.Log($"람다함수 {x}"); // 파라메터 하나인 람다식
        //Func<int> ccc = () => 10; // 10을 리턴하는 람다식


      
        //onDie += player.AddScore;
        //onDie += () => player.AddScore(score); // 죽을 때 플레이어의 AddScore함수에 파라메터로 score넣기

    }

    protected override void OnDisable()
    {
        if(player != null)
        {
            onDie -= PlayerAddScore; 
            onDie = null;
            player = null;
        }
        base.OnDisable();
    }


    /// <summary>
    /// 플레이어 점수 추가용 함수(델리게이트 등록용)
    /// </summary>
    void PlayerAddScore()
    {
        player.AddScore(score);
    }
    private void Update()
    {
        /*        VectorY += EnemySpeedY* Mathf.Cos( 90.0f * Mathf.Deg2Rad) * Time.deltaTime;

                EnemyVector = new Vector3(EnemyVector.x, VectorY, 0);
                Debug.Log(Time.deltaTime * EnemyVector);
                if(VectorY > MaxY ) 
                {
                    EnemySpeedY *= -1;
                }
                else if(VectorY < MinY ) 
                {
                    EnemySpeedY *= -1;

                }
                transform.Translate(Time.deltaTime * EnemyVector);*/


/*
        //elapsedTime += Time.deltaTime; // 시작부터 진행된 시간 측정
        elapsedTime += Time.deltaTime * frequency; //sin 그래프 속도증가

        transform.position = new Vector3(transform.position.x - Time.deltaTime*speed,
            spawnY+Mathf.Sin(elapsedTime) * amplitude, // sun그래프에 의해 높이변화
            0.0f);
*/

        OnMoveUpdate(Time.deltaTime);
    }


    /// <summary>
    /// 사망처리용 함수
    /// </summary>
    private void OnDie()
    {
        //Instantiate(enemyExplosion,transform.position, Quaternion.Euler(0, 0, UnityEngine.Random.value * 360f));
        Factory.Instance.GetExplosionEffect(transform.position);
/*        Player player = FindAnyObjectByType<Player>();
        player.EnemyObj(this.gameObject);*/
        //player.AddScore(score);
        onDie?.Invoke();

        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet")|| collision.gameObject.CompareTag("Player")) // 총알,플레이어 부딪치면 HP 1감소
        {
            Hp--;
        }
 
    }

    /// <summary>
    /// Enemy 계열의 초기화 함수
    /// </summary>
    private void OnInitialize()
    {
        if(player == null)
        {
            player = GameManager.Instance.Player;
        }

        if(player != null)
        {
            //onDie += () => player.AddScore(score); // 죽을 때 플레이어의 AddScore함수에 파라메터로 score넣기
            onDie += PlayerAddScore; // 플레이어 점수 증가 함수 등록
        }

        Hp = maxHP; // HP 최대로 설정
    }

    /// <summary>
    /// 업데이트 중에 이동 처리하는 함수
    /// </summary>
    /// <param name="deltaTime">프레임간의 간격</param>
    protected virtual void OnMoveUpdate(float deltaTime)
    {
        transform.Translate(deltaTime * speed * -transform.right, Space.World); // 기본 동작(왼쪽으로 이동)
    }

/*
    /// <summary>
    /// 시작 위치를 설정하는 함수
    /// </summary>
    /// <param name="position"></param>
    public void SetStartPosition(Vector3 position)
    {
        transform.position = position;
        spawnY = position.y;
    }*/
    // 실습 
    // 1. 적에게 HP 추가 (3대 맞으면 폭발)
    // 2. 적이 폭발할 때 explosionEffect 생성
}
