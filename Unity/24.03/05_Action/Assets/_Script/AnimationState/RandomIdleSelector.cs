using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIdleSelector : StateMachineBehaviour
{
    readonly int IdleSelect_Hash = Animator.StringToHash("IdleSelect");
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger(IdleSelect_Hash, RandomSelect());
    }

    /// <summary>
    /// 랜덤하게 0~4 사이의 값을 선택하는 함수 (수별로 확률이 다름)
    /// </summary>
    /// <returns>0~4</returns>
    int RandomSelect()
    {
        int select = 0; // 80%
        float num = Random.value;

        if (num < 0.05f)
        {
            select = 4; // 5%
        }
        else if (num < 0.1f)
        {
            select = 3; // 5%
        }
        else if (num < 0.15f)
        {
            select = 2; // 5%
        }
        else if (num < 0.2f)
        {
            select = 1; // 5%
        }

        return select;
    }
}
