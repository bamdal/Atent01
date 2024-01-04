using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        Destroy(this.gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }


}
