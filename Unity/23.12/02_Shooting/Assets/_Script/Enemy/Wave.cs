using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : EnemyBase
{
    public float amplitude = 3.0f; //높이제한

    public float frequency = 2.0f; // 사인 그래프가 황복하는데 걸리는 시간 증폭용

    float spawnY = 0.0f; // 적이 스폰된 높이
    float elapsedTime = 0.0f; // 전체 경과시간 frequency로 증폭함



    /// <summary>
    /// 업데이트 중에 이동 처리하는 함수
    /// </summary>
    /// <param name="deltaTime">프레임간의 간격</param>
    protected override void OnMoveUpdate(float deltaTime)
    {
        elapsedTime += Time.deltaTime * frequency; //sin 그래프 속도증가

        transform.position = new Vector3(transform.position.x - Time.deltaTime * speed,
            spawnY + Mathf.Sin(elapsedTime) * amplitude, // sun그래프에 의해 높이변화
            0.0f);
    }

    
        /// <summary>
        /// 시작 위치를 설정하는 함수
        /// </summary>
        /// <param name="position"></param>
        public void SetStartPosition(Vector3 position)
        {
            transform.position = position;
            spawnY = position.y;
        }
    // 실습 
    // 1. 적에게 HP 추가 (3대 맞으면 폭발)
    // 2. 적이 폭발할 때 explosionEffect 생성
}
