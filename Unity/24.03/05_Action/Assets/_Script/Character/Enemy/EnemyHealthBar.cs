using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    // 적의 HP 변경시 남은 HP 비율로 표시
    // 2. 색상 정상적으로 보이게 하기
    // HP는 왼쪽으로 줄어드는 듯이 보여야 한다
    // 빌보드여야 한다. - 항상 카메라를 정면으로 바라보아야 한다 

    /// <summary>
    /// fill의 피봇이 될 트랜스폼
    /// </summary>
    Transform fillPivot;
    private void Awake()
    {
        fillPivot = transform.GetChild(1);  // fill 피봇 찾기

        IHealth target = GetComponentInParent<IHealth>();
        target.onHealthChange += Refresh;   // 부모에서 IHealth찾아서 델리게이트 연결
    }


    private void LateUpdate()
    {
        // 빌보드로 만들기 위해 카메라의 정면으로 보이게 하기
        transform.forward =  transform.position - Camera.main.transform.position ;
    }

    /// <summary>
    /// 부모의 HP가 변경되면 실행되는 함수
    /// </summary>
    /// <param name="ratio">HP 비율 (hp/maxHP)</param>
    private void Refresh(float ratio)
    {
        fillPivot.localScale = new(ratio, 1, 1);
    }
}
