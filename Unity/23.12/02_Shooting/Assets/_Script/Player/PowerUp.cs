using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUp : RecycleObject
{
    // 파워업 아이템
    // 플레이어가 먹었을때
    // - 1단계에서 먹었을 때 : 총알이 2개씩 나간다.
    // - 2단계에서 먹었을 때 : 총알이 3개씩 나간다.
    // - 3단계에서 먹었을 떄 : 보너스 점수가 1000점 올라간다
    // 파워업 아이템은 랜덤한 방향으로 움직인다.
    // - 일정한 시간 간격으로 이동 방향이 변경된다.
    // - 높은 확률로 플레이어 반대쪽 방향을 선택한다.

    /*  float speed = 3.0f;
      float angle = 20.0f;
      float doubleangle;
      float time = 0;

      Player player;

      private void Awake()
      {
          doubleangle = angle*2;
          player = FindAnyObjectByType<Player>();
          if (player.transform.position.y - transform.position.y<0 )
          {
              doubleangle *= -1;
              angle *=-1;
          }
          else
          {

          }
          transform.Rotate(angle * Vector3.forward);
      }

      private void Update()
      {
          time += Time.deltaTime;   
          transform.Translate(Time.deltaTime * speed * -Vector3.right);
          if( time > 2.0f ) 
          {
              time = 0.0f;
              doubleangle *= -1;
              transform.Rotate(doubleangle * Vector3.forward);
          }

      }
  */

    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed = 2.0f;
    // 움직시는 텀
    public float dirChangeInterval = 1.0f;

    /// <summary>
    /// 현재이동방향
    /// </summary>
    Vector3 dir;

    Transform playerTransform;

    /// <summary>
    /// 방향전환이 가능한 횟수(최대치)
    /// </summary>
    public int dirChangeCountMax = 5;

    /// <summary>
    /// 남아있는 방향전환 횟수
    /// </summary>
    int dirChangeCount = 5;

    public int DirChangeCount
    {
        get => dirChangeCount;
        set
        {

            /*            if (value != dirChangeCount)
                        {
                            dirChangeCount = value;

                            if (dirChangeCount < 0)
                            {
                                gameObject.SetActive(false);
                            }

                            dirChangeCount = Mathf.Clamp(dirChangeCount, 0, dirChangeCountMax);


                        }*/
            dirChangeCount = value;                         // 값을 변경시키고
            animator.SetFloat("Count", dirChangeCount);     // 애니메이터의 파라메터 수정

            StopAllCoroutines();                            // 이전에 돌아가던 코루틴 종료(벽에 부딪쳤을때 필요)
            if (dirChangeCount > 0 && gameObject.activeSelf)// 방향전환 회수가 남아있으면 실행
            {
             
                StartCoroutine(DirectiomChange());          // 방향전환 코루틴 실행
            }


        }

    }

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        StopAllCoroutines(); // 코루틴 초기화
        DirChangeCount = dirChangeCountMax;
        playerTransform = GameManager.Instance.Player.transform;
        dir = Vector3.zero; // 방향 초기화

    }


    /// <summary>
    /// 주기적으로 방향 전환 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator DirectiomChange()
    {

        yield return new WaitForSeconds(dirChangeInterval);
        // 약 70% 확률로 플레이어 반대로 감
        if (Random.value < 0.4f)
        {
            // 플레이어 반대방향
            Vector2 playerToPowerUp = transform.position - playerTransform.position; // 방향 벡터 구하고
            dir = Quaternion.Euler(0, 0, Random.Range(-90.0f, 90.0f)) * playerToPowerUp; // +-90도 사이로 회전한 값으로 이동

        }
        else
        {
            // 모든 방향이니 50%확률로 플레이어 반대방향
            dir = Random.insideUnitCircle;   // 반지름 1짜리 원내부의 랜덤지점으로 가는 방향 저장

        }
/*        animator.SetFloat("PowerUpAnimationSpeed", dirChangeCountMax / Mathf.Clamp(DirChangeCount, 1, dirChangeCountMax));
        DirChangeCount--;*/
        dir.Normalize();                 // 구한 방향의 크기를 1로 지정
        DirChangeCount--;
    }

    private void Update()
    {
        transform.Translate(Time.deltaTime * moveSpeed * dir);  // 항상 dir방향으로 이동
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Border") && DirChangeCount > 0)   // 보더랑 부딪치면
        {
            dir = Vector2.Reflect(dir, collision.contacts[0].normal);   // 이동 방향 반사
            DirChangeCount--;
        }
    }

}

// 실습 
// 1. 파워업 아이템은 최대 회수만큼만 방향전환을 할 수 있다(벽에 부딪치는것도 1회로 취급)
// 2. 애니메이터를 이용해서 남아있는 방향전환 회수에 비례해서 빠르게 깜박인다.
// 