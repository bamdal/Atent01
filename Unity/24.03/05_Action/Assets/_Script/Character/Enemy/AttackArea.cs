using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

//공격 2번 맞는 버그 수정용
public class AttackArea : MonoBehaviour
{
    public Action<IBattler> onPlayerIn;
    public Action<IBattler> onPlayerOut;

    /// <summary>
    /// 공격 범위를 결정하는 콜라이더
    /// </summary>
    public SphereCollider attackArea; // 실행전에도 기즈모로 공격 범위를 보여주기 위해 public으로 함

    private void Awake()
    {
        attackArea = GetComponent<SphereCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        // 추적상태일때 플레이어가 들어오면
        if ( other.CompareTag("Player"))
        {
            IBattler target = other.GetComponent<IBattler>();
            onPlayerIn?.Invoke(target); // 플레이어가 들어왔음을 알림
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IBattler target = other.GetComponent<IBattler>();
            onPlayerOut?.Invoke(target);    // 플레이어가 나갔음을 알림
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        if (attackArea != null)
        {
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(transform.position, transform.up, attackArea.radius, 5);
        }

    }
#endif
}
