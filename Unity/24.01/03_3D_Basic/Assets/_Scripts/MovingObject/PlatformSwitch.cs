using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSwitch : PlatformBase, IInteracable
{
    /// <summary>
    /// 플랫폼이 움직일지 멈출지를 결정하는 변수
    /// </summary>
    bool isMoving = false;

    public bool CanUse => false;

    private void Start()
    {
        Target = targetWayPoints.GetNextWaypoint(); // 시작했을 때 플랫폼이 안움직이는 문제해결용(올라가자 마자 도착하는 문제)
    }

    protected override void OnMove()
    {
        if (isMoving)   // isMoving이 ture일 때만 움직임
        {
            base.OnMove();

        }
    }

    protected override void OnArrived()
    {
        isMoving = false;   // 도착하면 멈추기
        base.OnArrived();

    }



    public void Use()
    {
        isMoving = true;
    }
}
