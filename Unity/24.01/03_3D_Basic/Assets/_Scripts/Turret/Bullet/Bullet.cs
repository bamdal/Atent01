using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : RecycleObject
{
    /// <summary>
    /// 총알의 속도
    /// </summary>
    public float initalSpeed = 20.0f;

    /// <summary>
    /// 총알의 수명
    /// </summary>
    public float lifeTime = 10.0f;

    Rigidbody rigid;

    Coroutine life;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(rigid.velocity.sqrMagnitude> 0.0f)
            transform.forward = rigid.velocity;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        life =StartCoroutine(LifeOver(lifeTime));                 // 수명 설정
        rigid.angularVelocity = Vector3.zero;               // 이전의 회전력 제거
        rigid.velocity = initalSpeed * transform.forward;   // 발사 방향과 속도 설정
    }


    private void OnCollisionEnter(Collision collision)
    {
        StopCoroutine(life);
        StartCoroutine(LifeOver(2.0f));                     // 충돌하고 2초 뒤에 사라짐
    }



}
