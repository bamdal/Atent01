using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAndSkill : StateMachineBehaviour
{
    Player player;
    readonly int Attack_Hash = Animator.StringToHash("Attack");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
        {
            player = GameManager.Instance.Player;
        }

        player.ShowWeaponEffect(true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.ShowWeaponEffect(false);
        animator.ResetTrigger(Attack_Hash);
    }


}
