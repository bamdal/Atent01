using System.Collections;
using TMPro;
using UnityEngine;

public class DoorManual : DoorBase, IInteracable
{
    /// <summary>
    /// 사용가능 여부, 쿨타임이 0 미만일 때 사용 가능
    /// </summary>
    public bool CanUse => currentCoolTime < 0.0f;

    TextMeshPro text; //3D글자(UI아님)

    /// <summary>
    /// 문이 열려있는 상태(true면 문이열림, false면 문이 닫혀있다.)
    /// </summary>
    bool isOpen = false;

    /// <summary>
    /// 재사용 쿨타임
    /// </summary>
    public float coolTime = 0.5f;
    /// <summary>
    /// 현재 남아있는 쿨타임
    /// </summary>
    float currentCoolTime = 0;

    protected override void Awake()
    {
        base.Awake();
        text = GetComponentInChildren<TextMeshPro>(true);
        text.enabled = false;
    }

    private void Update()
    {
        currentCoolTime -= Time.deltaTime;
    }

    public void Use()
    {
        if(CanUse) // 사용가능할때 만 사용
        {
            if (isOpen)
            {
                Close();
                isOpen = false;
            }
            else
            {
                Open();
                isOpen = true;
            }
            currentCoolTime = coolTime; // 쿨타임 초기화
        }

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        { 
           
            Vector3 cameraToDoor = transform.position - Camera.main.transform.position;
            
            float angle = Vector3.Angle(transform.forward, cameraToDoor);
            if(angle > 90.0f)
            {
                text.transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0); // 문의 회전에서 Y축으로 반바퀴 더 돌리기

            }
            else
            {
                text.transform.rotation = transform.rotation; // 문의 회전 그대로 적용
            }

            //text.transform.forward = Camera.main.transform.forward;

            text.enabled = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        { 
            text.enabled = false;
        
        }
            
    }

}

// 실습
// 1. 일정시간 이후에 자동으로 문이 닫히기
// 2. 플레이어가 자신의 트리거 안에 들어오면 글자로 단축키 보여주기