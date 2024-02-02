using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorKey : MonoBehaviour
{

    public float rotationSpeed = 60.0f;
    Transform modelTransform;

    public Action onConsume;

    private void Awake()
    {
        modelTransform = transform.GetChild(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            OnConsume();
        }
    }

    /// <summary>
    /// 이 열쇠를 획득했을 때 일어날 일을 처리하는 함수
    /// </summary>
    protected virtual void OnConsume()
    {

        onConsume?.Invoke();
        Destroy(this.gameObject);
    }

    private void Update()
    {
        modelTransform.Rotate(Time.deltaTime * rotationSpeed * transform.up);
    }

/*    private void OnValidate()
    {
        ResetTarget(target);
    }

    void ResetTarget(DoorBase door)
    {
        if(door != null)
        {
            target = door;
            //target = door as DoorAuto; // door가 DoorAuto로 캐스팅 될 수 있으면 캐스팅하고 아니면 null
            // is : is 왼쪽에 있는 변수의 데이터 타입이 오른쪽에 있는 타입이나 그 타입을 상속받은 타입이면 true 아니면 false
            // as : as 왼쪽에 있는 변수의 데이터 타입이 오른쪽에 있는 타입이나 그 타입을 상속받은 타입이면 
            //          캐스팅을 해서 null이 아닌 값을 리턴하고, 아니면 null이다
        }
    }*/
}

// 이 오브젝트와 플레이어가 닿으면 target에 지정된 문이 열린다.
// 이 오브젝트와 플레이어가 닿으면 이 오브젝트는 사라진다.
// 이 오브젝트는 제자리에서 빙글빙글 돈다

// 잠금해제용 문과 열쇠 만들기
// DoorAutoLock : DoorAuto 상속받은 클래스 평소에는 잠겨있다가 이문의 열쇠를 먹으면 문열수있다.
// 잠금상태와 해제상태 문 색상이 다름
// UnlockDoorKey : 먹으면 연결된 DoorAutoLock 잠금해제
