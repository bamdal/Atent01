using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LifeTimeGauge : MonoBehaviour
{
    // 플레이어의 수명을 슬라이더로 표현하기
    // 플레이어의 남은 수명을 text로 소수점 한자리까지 표현하기
    Player player;
    float maxValue = 1.0f;

    Slider slider;

    TextMeshProUGUI text;
    private void Awake()
    {
        slider = GetComponent<Slider>();
        text = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        player = GameManager.Instance.Player;


    }
    private void Start()
    {
        player.onLifeTimeChange += Refresh;
        maxValue = player.startLifeTime;

    }

    private void Refresh(float ratio)
    {
        //text.text =$"{(ratio*maxValue).ToString("F1")} Sec";
        text.text =$"{(ratio*maxValue):f1} Sec";
        slider.value = ratio;


    }

    private void Update()
    {

    }
}
