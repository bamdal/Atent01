using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    //시작하자마자 오른쪽으로 초속 7로 움직이게 만들기
    Vector3 bulletVector = Vector3.right;
    public float bulletSpeed = 7.0f;
    bool BV = true;
    float i = 0;
    private void OnEnable()
    {
        
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
}
