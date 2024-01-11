using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : RecycleObject
{
    //시작하자마자 오른쪽으로 초속 7로 움직이게 만들기
    Vector3 bulletVector = Vector3.right;
    public float bulletSpeed = 7.0f;
    public GameObject HitEffectPrefeb;

    public float LifeTime = 3.0f;
    //bool BV = true;
    //float i = 0;
/*    private void OnEnable()
    {
        //Destroy(this.gameObject,LifeTime);
       
    }*/

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(LifeOver(LifeTime));
    }
    // Update is called once per frame
    void Update()
    {
        // 벡터는 스칼라(힘의 크기)와 방향으로 이뤄져 있다
        // 벡터중에 스칼라가 1인 벡터를 Unit벡터라 한다
        // 벡터의 크기를 1로 만드는 작업을 정규화라고 한다
        // => 정규화 된 벡터는 순수하게 방향만 나타낸다.
        // 정규화는 벡터를 벡터의 스칼라 값으로 나눈다.
/*        if(BV) 
        {
            i++;
            bulletVector += new Vector3(0, 1, 0); 
            if (i > 1) { 
                
                BV = false;
            }
        }
        else 
        {
            i--;
            bulletVector += new Vector3(0, -1, 0);
            if (i < -1) 
            {
                
                BV = true;
            }
        }
        */
        
        transform.Translate(Time.deltaTime * bulletSpeed * bulletVector); // time * speed *(x,y)  3번 곱셈
       // transform.Translate(bulletVector *Time.deltaTime  * bulletSpeed); // (x,y) * time * speed 4번 곱셈
        // 벡터의 계산은 마지막으로
    }
    // 실습
    // 1. bullet 프리펩에 필요한 컴포넌트 추가하고 설정
    // 2. 총알은 "Enemy"태그를 가진 오브젝트와 부딪치면 부딪친대상 삭제
    // 3. 총알은 다른 오브젝트와 부딪치면 자기 자신을 삭제
    // 4. Hit 스프라이트를 이용해 HitEffect 프리펩 생성 
    // 5. 총알이 부딪친 위치에 HitEffect 생성
    // 6. HitEffect는 한번만 재생
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //  if (collision.gameObject.CompareTag("Enemy")) // collision의 게임 오브젝트가 "Enemy"라는 태그를 가지는지 확인하는 함수
        //  {                                             // .tag == "Enemy"는 매우 비효율적

        //   /*GameObject obj =*/ Instantiate(HitEffectPrefeb,transform.position,Quaternion.Euler(0,0,Random.value*360f)); 
        // hit 이펙트 생성
        // Destroy(obj, obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        //Destroy(this.gameObject);
        //  } 

        Factory.Instance.GetHitEffect(transform.position);
        gameObject.SetActive(false);

    }
}
