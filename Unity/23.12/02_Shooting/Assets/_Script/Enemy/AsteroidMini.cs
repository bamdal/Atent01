using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidMini : EnemyBase
{


    // 이동속도가 기본속도에서 랜덤으로 +-1
    // 회전 속도 랜덤
    // 방향도 설정이 되어야 한다.
    [Header("작은 운석 data")]
    /// <summary>
    /// 기준 속도(첫 speed값)
    /// </summary>
    float baseSpeed;

    /// <summary>
    /// 회전 속도(0~360)
    /// </summary>
    float rotateSpeed;

    /// <summary>
    /// 이동 방향
    /// </summary>
    Vector3? direction = null;

    /// <summary>
    /// 이동 방향 읽고 쓰기 위한 프로퍼티
    /// </summary>
    public Vector3 Direction
    {
        private get => direction.GetValueOrDefault();   // direction이 null이면 Vector.Zero 리턴, 아니면 값을 리턴
        set
        {
            if(direction == null)   // 쓰기는 (재활용 될 때마다) 한번만 가능하도록 설정
            {
                direction = value.normalized;

            }
        }
    }

    private void Awake()
    {
        baseSpeed = moveSpeed;  // 첫 속도를 기준 속도로 지정
    }


    /// <summary>
    /// 활성화 될 때마다 실행
    /// </summary>
    protected override void OnInitialize()
    {
        base.OnInitialize();    // 기본적으로 해야할 일 처리

        moveSpeed = baseSpeed + Random.Range(-1.0f, 1.0f);  // 이동 속도 랜덤하게 변경
        rotateSpeed = Random.Range(0, 360.0f);          // 회전 속도도 랜던하게 변견
        direction = null;                               // 방향을 null로 초기화
    }

    /// <summary>
    /// 이동 처리
    /// </summary>
    /// <param name="deltaTime">델타타입 - 프레임별 시간</param>
    protected override void OnMoveUpdate(float deltaTime)
    {
        transform.Translate(deltaTime * moveSpeed * Direction, Space.World);    // Direction 방향으로 이동 ( 초속 speed만큼)
        transform.Rotate(deltaTime * rotateSpeed * Vector3.forward);        // z축 기준으로 회전
    }
}
