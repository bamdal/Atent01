using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    public float moveSpeed = 20.0f;

    [Networked]
    TickTimer Life { get; set; }

    public void Init()
    {
        Life = TickTimer.CreateFromSeconds(Runner, 5.0f);   // life는 5초를 카운팅한다
    }

    public override void FixedUpdateNetwork()
    {
        if (Life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            transform.position += Runner.DeltaTime * moveSpeed * transform.forward; // 앞으로 계속 이동

        }

        
    }
}
