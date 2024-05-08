using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    /// <summary>
    /// 현재 턴 번호(몇번째 턴인지)
    /// </summary>
    int turnNumber = 1;

    /// <summary>
    /// 턴 타임아웃되는데 걸리는 시간
    /// </summary>
    public float turnDuration = 5.0f;

    /// <summary>
    /// 타임 아웃 시간 확인용 프로퍼티
    /// </summary>
    public float TurnDuration => turnDuration;

    /// <summary>
    /// 이번 턴이 타임아웃 될 때 까지 남아있는 시간
    /// </summary>
    float turnRemainTime = 0.0f;

    /// <summary>
    /// 타임아웃이 활성화되어 있는지 표시(false면 타임아웃)
    /// </summary>
    public bool TimeOutEnable =true;

    /// <summary>
    /// 현재 턴 진행상황 표시용
    /// </summary>
    enum TurnProcessState 
    {
        None = 0,   // 행동을 완료한 사람이 없음
        One,        // 한명만 행동을 완료함
        Both        // 둘 다 행동을 완료함
    }

    /// <summary>
    /// 현재 턴 진행상황
    /// </summary>
    TurnProcessState state = TurnProcessState.None;

    /// <summary>
    /// 턴이 진행될지 여부(true면 턴이 지속되고 false면 턴이 진행되지 않는다)
    /// </summary>
    bool isTurnEnable = true;

    /// <summary>
    /// 턴이 시작됨을 알리는 델리게이트(int : 시작된 턴 번호)
    /// </summary>
    public Action<int> onTurnStart;

    /// <summary>
    /// 턴이 끝났음을 알리는 델리게이트
    /// </summary>
    public Action onTurnEnd;

    /// <summary>
    /// 턴 종료 처리 중인지 확인하는 변수
    /// </summary>
    bool isEndProcess = false;




    public void OnInitialize(PlayerBase user, PlayerBase enemy)
    {
        turnNumber = 0;     // OnTurnStart에서 trunNumber를 증가시키기에 0으로 초기화

        if (!TimeOutEnable)
        {
            turnDuration = float.MaxValue;  // turnDuration을 매우 길게 잡기
        }
        turnRemainTime = TurnDuration;      // 턴 남은 시간을 turnDuration으로 설정

        state = TurnProcessState.None;      // 턴 진행 상태 초기화
        isTurnEnable = true;                // 턴 켜기

        onTurnStart = null;                 // 델리게이트 초기화
        onTurnEnd = null;

        if(user != null)                    // user가 있으면 행동이 끝났거나 패배했을 때 실행될 함수 연결
        {
            user.onActoinEnd += PlayerTurnEnd;
            user.onDefeat += TurnStop;
        }

        if(enemy != null)                    // enmey가 있으면 행동이 끝났거나 패배했을 때 실행될 함수 연결
        {
            enemy.onActoinEnd += PlayerTurnEnd;
            enemy.onDefeat += TurnStop;
        }


        OnTurnStart();                      // 턴 시작
    }



    private void Update()
    {
        turnRemainTime -= Time.deltaTime;
        if (isTurnEnable && turnRemainTime < 0.0f)
        {
            OnTurnEnd();
        }
    }
    void OnTurnStart()
    {
        if (isTurnEnable)
        {
            turnNumber++;
            Debug.Log($"{turnNumber}턴 시작");
            state = TurnProcessState.None;  
            turnRemainTime = TurnDuration;

            onTurnStart?.Invoke(turnNumber);
        }
    }

    void OnTurnEnd()
    {
        if(isTurnEnable)
        {
            isEndProcess = true;
            onTurnEnd?.Invoke();
            Debug.Log($"{turnNumber}턴 종료");

            isEndProcess = false;
            OnTurnStart();
        }
    }


    void PlayerTurnEnd()
    {
        if (!isEndProcess)
        {
            state++;
            if (state >= TurnProcessState.Both)
            {
                OnTurnEnd();
            }
        }
    }

    private void TurnStop(PlayerBase _)
    {
        isTurnEnable = false;
    }
}
