using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillArea : MonoBehaviour
{
    // 스킬을 사용하면 활성화
    // 스킬을 끄면 비활성화

    /// <summary>
    /// 스킬이 데미지를 주는 간격
    /// </summary>
    public float skillTick = 0.5f;

    /// <summary>
    /// 플레이어의 공격력을 증폭시키는 용도 (0.2 = 20%증가)
    /// </summary>
    public float skillPower = 0.2f;

    /// <summary>
    /// 활성화 된 시점에서 스킬의 최종 공격력
    /// </summary>
    float finalPower; 


    public void Activate(float power)
    {
        finalPower = power * (1 + skillPower);
        gameObject.SetActive(true);
        // skillTick 마다 트리거 안에있는 모든 적에게 finalPower만큼 데미지를 준다
        // 칼의 이펙트 킨다
        // 활성화 되어있는 동안 지속적으로 플레이어의 MP가 감소
        // 스킬 애니메이션시작
    }

    public void Deactivate()
    {
        // 칼의 이펙트 끄고 비활성화
        // 스킬 애니메이션 종료
        gameObject.SetActive(false);    
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
