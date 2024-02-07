using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPush : TrapBase
{
    public float pushPower = 10.0f;
    Animator animator;
    readonly int ActivateHash = Animator.StringToHash("Activate");

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected override void OnTrapActivate(GameObject target)
    {
        animator.SetTrigger(ActivateHash);
        Rigidbody rb = target.GetComponent<Rigidbody>();
        if(rb != null )
        {
            Vector3 dir = (transform.up + transform.forward).normalized;
            rb.AddForce(dir*pushPower,ForceMode.Impulse);
        }
    }
}
