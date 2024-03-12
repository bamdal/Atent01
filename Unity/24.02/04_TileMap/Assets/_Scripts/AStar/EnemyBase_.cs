using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase_ : MonoBehaviour
{
    //컴포넌트 불러오기
    Rigidbody2D rb;

    /// <summary>
    /// 플레이어 위치 타게팅
    /// </summary>
    Vector2 targetPos;

    /// <summary>
    /// 좌우 체크용 인트
    /// </summary>
    public int checkLR = 1;

    /// <summary>
    /// 적 개체의 최대 체력
    /// </summary>
    public int maxHp = 10;

    /// <summary>
    /// 적 개체의 현재 체력
    /// </summary>
    private int hp = 10;

    /// <summary>
    /// 적 개체의 데미지 
    /// </summary>
    public int mobDamage = 0;

    /// <summary>
    /// 적 개체의 이동속도
    /// </summary>
    public float mobMoveSpeed = 0;

    /// <summary>
    /// Hp 프로퍼티
    /// </summary>
    public int Hp
    {
        get { return hp; }
        set
        {
            hp = value;
            hp = Mathf.Max(hp, 0);

            // Hp가 0 이하면 사망
            if (Hp <= 0)
            {
                Die();
            }

        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hp = maxHp;
    }

    protected virtual void Start()
    {
        //player = GameManager.Instantiate.Player;
    }

    protected virtual void Update()
    {
        // 플레이어의 위치를 받는다.
        // targetPos = 

        // 플레이어의 위치에 따라 스프라이트를 변경한다 ( 좌우 )
        if (targetPos.x < rb.position.x) checkLR = -1; // ( 왼쪽이면 - )
        else checkLR = 1;                              // 우측이면 +

    }

    /// <summary>
    /// 충돌을 검출하는 메서드
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAttack")) // 플레이어의 공격에 맞았다면
        {
            Damaged(1); // 데미지 함수를 실행시킨다.
        }
    }

    /// <summary>
    /// 피해를 받았을때 실행할 함수 생성
    /// </summary>
    /// <param name="Damage">플레이어에게 받은 피해</param>
    /// <exception cref="NotImplementedException"></exception>
    private void Damaged(int Damage)
    {
        Hp -= Damage;
    }


    /// <summary>
    /// 죽었을때 실행 될 메서드
    /// </summary>
    protected virtual void Die()
    {

    }
}
