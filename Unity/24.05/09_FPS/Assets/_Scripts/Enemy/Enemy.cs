using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 상태 관련 --------------------------------------------------------------------------------------------------------------

    public enum BehaviourState : byte
    {
        Wander = 0, // 배회상태, 주변을 계속 왔다갔다한다.
        Chase,      // 추적상태, 플레이어가 마지막으로 목격된장소를 향해 계속 이동한다.
        Find,       // 탐색상태, 추적 도주에 플레이어가 시야에서 사라지면 주변을 두리번 거리며 찾는다
        Attack,     // 공격상태, 플레이어가 일정범위안에 들어오면 일정주기로 공격
        Dead        // 사망상태, 죽어서 디스폰(일정 시간 후에 재생성)
    }

    BehaviourState state = BehaviourState.Dead;

    BehaviourState State
    {
        get => state;
        set
        {
            if (state != value)
            {
                OnStateExit(value);
                state = value;
                OnStateEnter(value);
            }
        }
    }

    /// <summary>
    /// 각 상태가 되었을 때 상태별 업데이트함수를 저장하는 델리게이트
    /// </summary>
    Action onUpdate = null;

    // HP 관련 --------------------------------------------------------------------------------------------------------------

    public float maxHp = 30.0f;

    [SerializeField]
    float hp = 30.0f;

    public float HP
    {
        get => hp;
        set 
        {
            hp = value; 
            if (hp <= 0)
            {
                State = BehaviourState.Dead;
            }
        }
    }

    void OnStateEnter(BehaviourState newState)
    {

    }

    void OnStateExit(BehaviourState newState)
    {

    }

    AttackSenser attackSenser;

    private void Awake()
    {
        attackSenser = GetComponent<AttackSenser>();
        attackSenser.onSensorTriggered += OnSensorTriggered;

        hp = maxHp;
    }

    private void OnSensorTriggered(GameObject _)
    {
        
    }

    // 이동 관련------------------------------------------------------------------------------------------------------------------------
}
