using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public enum SwitchState
{
    Idle,
    Open,
    Close
}

public class DoorSwitch : MonoBehaviour, IInteracable
{
    public GameObject target;
    IInteracable useTarget;
    Animator animator;

    bool IsUsing = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        if(target != null)
        {
            useTarget = target.GetComponent<IInteracable>();
        }

        if(useTarget == null)
        {
            Debug.LogWarning($"{gameObject.name}에게 사용할 오브젝트가 없음");
        }
    }

    /*    public void Use()
        {
            if(useTarget != null)
            {
                if(!IsUsing)
                {
                    StopAllCoroutines();
                    useTarget.Use();
                    StartCoroutine(ResetSwich());
                }
            }
        }

        IEnumerator ResetSwich()
        {
            IsUsing = true;
            animator.SetBool("IsOpen", true);
            float aniTime = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            yield return new WaitForSeconds(aniTime);
            animator.SetBool("IsOpen", false);
            yield return new WaitForSeconds(aniTime);
            IsUsing = false;
        }
    */

    public void Switch(SwitchState type)
    { 
        switch (type)
        {
            case SwitchState.Idle:

                break; 
            case SwitchState.Open:
                useTarget.Use();
                break;
            case SwitchState.Close:
                break;
            default:
                break;
        }
    }

    public void Use()
    {
        
    }
}

// 실습
// 1. DoorManual 새로 만들기
// 1.1 열렸을 때 사용하면 닫힌다.
// 1.2 닫혔을 때 사용하면 열린다.
// 2. DoorSwitch 수정하기
// 2.1 3개 상태를 가진다(idle,On,Off)
// 2.2 사용할 문은 무조건 Manual 계열의 문만 가능
// 2.3 on이 될 때 문이 열려야 한다.
// 2.4 off때 문이 닫힌다.(autoClose문은 시간지나면 닫히고 시간 다되기 전에 off가 되면 즉시 닫힘)