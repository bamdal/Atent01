using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
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
    public float amplitude = 3.0f; //높이제한

    public float frequency = 2.0f; // 사인 그래프가 황복하는데 걸리는 시간 증폭용

    float spawnY = 0.0f; // 적이 스폰된 높이
    float elapsedTime = 0.0f; // 전체 경과시간 frequency로 증폭함
    float maxHP =3.0f;
    float hp = 3.0f;
    /// <summary>
    /// 적을 해치웠을때 얻을 점수ㄴ
    /// </summary>
    public int score= 10;
    /// <summary>
    /// 적이 죽을때 실행될 델리게이트
    /// </summary>
    Action onDie;

    // 람다식, 람다함수(Lambda), 익명함수



    public GameObject enemyExplosion;

    public float Hp 
    { 
        get => hp;
        private set
        {
            if (hp != value)
            {
                Debug.Log("맞음");
            }

            hp = value;
            hp = Math.Clamp(value, 0.0f, MaxHP);
            if(hp ==0.0f) 
            {
               
                OnDie();
            }
        }
        
    }

    public float MaxHP { get => maxHP; set => maxHP = value; }

    private void Awake()
    {
/*        VectorY = transform.position.y;
        WorldY = VectorY;
        Mathf.Cos(Time.deltaTime*90.0f * Mathf.Deg2Rad); //90도를 라디안으로 변경해 Cos결과구하기*/
    }

    private void Start()
    {
        spawnY = transform.position.y;
        elapsedTime = 0.0f;

        Action aaa = () => Debug.Log("람다함수"); // 파라메터 없는 람다식
        Action<int> bbb = (x) => Debug.Log($"람다함수 {x}"); // 파라메터 하나인 람다식
        Func<int> ccc = () => 10; // 10을 리턴하는 람다식


        Player player = FindAnyObjectByType<Player>();
        //onDie += player.AddScore;
        onDie += () => player.AddScore(score); // 죽을 때 플레이어의 AddScore함수에 파라메터로 score넣기
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



        //elapsedTime += Time.deltaTime; // 시작부터 진행된 시간 측정
        elapsedTime += Time.deltaTime * frequency; //sin 그래프 속도증가

        transform.position = new Vector3(transform.position.x - Time.deltaTime*speed,
            spawnY+Mathf.Sin(elapsedTime) * amplitude, // sun그래프에 의해 높이변화
            0.0f);

    }



    private void OnDie()
    {
        Instantiate(enemyExplosion,transform.position, Quaternion.Euler(0, 0, UnityEngine.Random.value * 360f));
        
/*        Player player = FindAnyObjectByType<Player>();
        player.EnemyObj(this.gameObject);*/
        //player.AddScore(score);
        onDie?.Invoke();

        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Hp--;
        }
    }
    // 실습 
    // 1. 적에게 HP 추가 (3대 맞으면 폭발)
    // 2. 적이 폭발할 때 explosionEffect 생성
}
