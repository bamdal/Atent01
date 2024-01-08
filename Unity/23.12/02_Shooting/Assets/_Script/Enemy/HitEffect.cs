using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    float animLength = 0.0f;
    private void Awake()
    {
        //Time.timeScale = 0.1f; // 1.0f 가 정상속도 0.1f면 10배 느리게
        // 애니메이터에서 클립길이 받아오기
        // GetCurrentAnimatorClipInfo(0) : 애니메이터의 첫번째 레이어의 클립 정보들 받아오기
        // GetCurrentAnimatorClipInfo(0)[0] : 애니메이터의 첫번째 레이어에 있는 애니메이션 클립중 첫번째 클립의 정보 받아오기
        animator = GetComponent<Animator>();
        animLength = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

        Destroy(this.gameObject, animLength);
    }


}
