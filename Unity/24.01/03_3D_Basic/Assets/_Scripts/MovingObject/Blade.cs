using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : WayPointUser
{
    public float spinSeeed = 720.0f;
    Transform bladeMesh;

    protected override Transform Target
    {
        set
        {
            base.Target = value;
            transform.LookAt(Target);
        }
    }

    private void Awake()
    {
        bladeMesh = transform.GetChild(0);
       
    }



    private void Update()
    {
        bladeMesh.Rotate(Time.deltaTime * spinSeeed * Vector3.right);
    }

    private void OnCollisionEnter(Collision collision)
    {
        IAlive alive = collision.gameObject.GetComponent<IAlive>();
        if (alive != null)
        {
            alive.Die();
        }
    }
}

// 1. Blade : 웨이포인트 사용했을 때 문제점 수정
// 2. WayPoints : GetNextWaypoint 구현
// 3. WayPointUser : Target 프로퍼티 구현, IsArrived 프로퍼티 구현, OnMove 함수 구현
// 4. PlatformBase 만들기 : 특정 두 지점을 계속 왕복하는 바닥(플레이어가 탑승했을 때 이동이 자연스럽게)
