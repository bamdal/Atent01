using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;



public class DoorSwitch : MonoBehaviour, IInteracable
{
    // 수동문을 열고 닫는 스위치
    enum State
    {
        Off,    // 스위치가 꺼진 상태
        On      // 스위치가 켜지 상태
        
    }

    /// <summary>
    /// 스위치의 현재 상태
    /// </summary>
    State state = State.Off;


    /// <summary>
    /// target이 가지고 있는 DoorManual
    /// </summary>
    public DoorManual targetDoor;

    /// <summary>
    /// 스위치 애니메이터
    /// </summary>
    Animator animator;

    /// <summary>
    /// 애니메이터용 해쉬
    /// </summary>
    readonly int SwitchOnHash = Animator.StringToHash("SwitchOn");

    void Start()
    {
        animator = GetComponent<Animator>();


        if (targetDoor == null)
        {
            Debug.LogWarning($"{gameObject.name}에게 사용할 문이 없음");
        }
    }


    /// <summary>
    /// 스위치 사용
    /// </summary>
    public void Use()
    {
        if (targetDoor != null) //조작할 문이 있어야 한다
        {
            switch (state)
            {
                case State.Off:
                    //스위치를 켠 상황
                    targetDoor.Open();
                    animator.SetBool(SwitchOnHash, true);
                    state = State.On;
                    break;
                case State.On:
                    // 스위치를 끄려는 상황
                    targetDoor.Close();
                    animator.SetBool(SwitchOnHash, false);
                    state = State.Off;
                    break;


            }
        }
    }




}

// 실습
// 1. DoorManual 새로 만들기
// 1.1 열렸을 때 사용하면 닫힌다.
// 1.2 닫혔을 때 사용하면 열린다.
// 2. DoorSwitch 수정하기
// 2.1 3개 상태를 가진다(On,Off)
// 2.2 사용할 문은 무조건 Manual 계열의 문만 가능
// 2.3 on이 될 때 문이 열려야 한다.
// 2.4 off때 문이 닫힌다.(autoClose문은 시간지나면 닫히고 시간 다되기 전에 off가 되면 즉시 닫힘)