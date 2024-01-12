using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Asteroid : RecycleObject
{
/*    public float speed = 2.0f;
    float spawnY = 0.0f;
    Vector3 spawnPos = Vector3.zero;
    private void Update()
    {
        //transform.Rotate(0, 0, 1f);
        transform.Translate(Time.deltaTime * speed *spawnPos);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
      
        spawnY = transform.position.y;

    }

    protected override void OnDisable()
    {
        base.OnDisable();
    

    }

    public void SetStartPosition(Vector3 position)
    {
        transform.position = position;
        spawnPos = position;
        spawnY = position.y;
    }*/
    
    Vector3 direction = Vector3.zero;
    public float moveSpeed = 3.0f;
    public float RotationSpeed = 360.0f;


    public void SetDestination(Vector3 destination)
    {
        direction = (destination - transform.position).normalized;

    }

    private void Update()
    {
        transform.Translate(Time.deltaTime * moveSpeed * direction,Space.World); // 월드기준으로 이동
        transform.Rotate(0,0,Time.deltaTime*RotationSpeed);
    }

    protected override void OnEnable()
    {

        base.OnEnable();
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + direction); // 진행방향 표시
    }
    // 실습
    // 1. 운석은 만들어졌을때 지정된 방향으로 움직인다.
    // 2. 운석은 계속 회전해야 한다.
    // 3. 운석은 오브젝트 풀에서 관리되어야 한다.




    // 스포너에서 생성할때 풀에서 만들기
}
