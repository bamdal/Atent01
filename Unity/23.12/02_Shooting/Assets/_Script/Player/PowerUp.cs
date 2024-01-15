using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // 파워업 아이템
    // 플레이어가 먹었을때
    // - 1단계에서 먹었을 때 : 총알이 2개씩 나간다.
    // - 2단계에서 먹었을 때 : 총알이 3개씩 나간다.
    // - 3단계에서 먹었을 떄 : 보너스 점수가 1000점 올라간다
    // 파워업 아이템은 랜덤한 방향으로 움직인다.
    // - 일정한 시간 간격으로 이동 방향이 변경된다.
    // - 높은 확률로 플레이어 반대쪽 방향을 선택한다.

    float speed = 3.0f;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        
        }
    }
}
