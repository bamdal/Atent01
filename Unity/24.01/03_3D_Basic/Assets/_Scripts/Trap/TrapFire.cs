using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapFire : TrapBase
{
    ParticleSystem Fire;

    private void Start()
    {
        Fire = transform.GetChild(1).GetComponent<ParticleSystem>();
    }

}
