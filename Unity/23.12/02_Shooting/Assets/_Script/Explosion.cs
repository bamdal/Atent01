using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Destroy(this.gameObject, animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
    }
}
