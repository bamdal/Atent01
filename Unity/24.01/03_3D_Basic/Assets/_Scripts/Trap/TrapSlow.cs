using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSlow : TrapBase
{
    Player player;
    CapsuleCollider collider;
    private void Start()
    {
        player = GameManager.Instance.Player;
        collider = GetComponent<CapsuleCollider>();
    }


    IEnumerator Slow()
    {
        float playerMoveSpeed;
        float playerRotaeSpeed;
        playerMoveSpeed = player.moveSpeed;
        playerRotaeSpeed = player.rotateSpeed;

        player.moveSpeed *= 0.3f;
        player.rotateSpeed *= 0.3f;
        yield return new WaitForSeconds(1.0f);

        player.moveSpeed = playerMoveSpeed;
        player.rotateSpeed = playerRotaeSpeed;
        collider.enabled = true;
    }
}
