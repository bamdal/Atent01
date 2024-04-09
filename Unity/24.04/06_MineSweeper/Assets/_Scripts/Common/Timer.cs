using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    /// <summary>
    /// 초 단위로 시간이 변경될 때 실행될 델리게이트
    /// </summary>
    public Action<int> onTimeChange;

    /// <summary>
    /// 시간 측정 시작한 이후의 경과 시간
    /// </summary>
    float elapsedTime;

    /// <summary>
    /// 시간 경과 확인용 프로퍼티
    /// </summary>
    public float ElapsedTime => elapsedTime;

    /// <summary>
    /// UI에 보여줄 시간(델리게이트 전달 및 변화 확인용)
    /// </summary>
    int displayTime = -1;

    int DisplayTime
    {
        get => displayTime;
        set
        {
            if(displayTime != value)
            {
                displayTime = value;
                onTimeChange?.Invoke(displayTime);
            }
        }
    }

    /// <summary>
    /// 시간 측정용 코루틴을 저장한 변수
    /// </summary>
    IEnumerator timeCoroutine;

    private void Start()
    {
        GameManager manager = GameManager.Instance;
        manager.onGameReady += TimerReset;
        manager.onGamePlay += TimerReset;
        manager.onGamePlay += Play;
        manager.onGameClear += Stop;
        manager.onGameOver += Stop;

        DisplayTime = 0;
        timeCoroutine = TimeProcess();
    }


    /// <summary>
    /// 타이머의 시간 측정을 시작하는 함수
    /// </summary>
    void Play()
    {
        StartCoroutine(timeCoroutine);
    }
    
    /// <summary>
    /// 타이머의 시간 측정을 정지하는 함수
    /// </summary>
    void Stop() 
    {
        StopCoroutine(timeCoroutine);
    }

    /// <summary>
    /// 저장된 시간을 초기화 하는 함수
    /// </summary>
    void TimerReset()
    {
        elapsedTime = 0;
        DisplayTime = 0;
        StopCoroutine(timeCoroutine);
    }

    /// <summary>
    /// 시간 측정하는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator TimeProcess()
    {
        while (true)
        {
            elapsedTime += Time.deltaTime;
            DisplayTime = (int)ElapsedTime; 
            yield return null;
        }
    }
}
