using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve : EnemyBase
{
    [Header("커브를 도는 적 데이터")]
    /// <summary>
    /// 회전 속도
    /// </summary>
    public float rotateSpeed = 15.0f;

    /// <summary>
    /// 회전방향 (1 = 반시계방향 -1 = 시계방향)
    /// </summary>
    float curveDirection = 1.0f;




    protected override void OnMoveUpdate(float deltaTime)
    {
        base.OnMoveUpdate(deltaTime);
        transform.Rotate(deltaTime * rotateSpeed * curveDirection * Vector3.forward); // 회전 추가
    }

    /// <summary>
    /// 현재 높이에 따라 회전 방향을 갱신하는 함수
    /// </summary>
    public void RefreshRotateDirection()
    {
        if (transform.position.y < 0)
        {
            curveDirection = -1.0f; // 우회전
        }
        else
        {
            curveDirection = 1.0f; // 좌회전
        }

    }



}
