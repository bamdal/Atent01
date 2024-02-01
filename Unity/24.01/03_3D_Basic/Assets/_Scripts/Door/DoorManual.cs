using System.Collections;
using TMPro;
using UnityEngine;

public class DoorManual : DoorBase, IInteracable
{
    public float autoCloseTime = 3.0f;

    TextMeshPro text; //3D글자(UI아님)

    private void Start()
    {
        text = GetComponentInChildren<TextMeshPro>(true);
        text.enabled = false;
    }
    public void Use()
    {
        Open();
        StopAllCoroutines();
        StartCoroutine(AutoClose());
    }
    IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(autoCloseTime);
        Close();
    }

    private void OnTriggerEnter(Collider other)
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

    private void OnTriggerExit(Collider other)
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