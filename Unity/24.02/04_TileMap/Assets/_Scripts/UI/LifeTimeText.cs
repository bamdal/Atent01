using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LifeTimeText : MonoBehaviour
{
    // 플레이어 수명에 따라 슬라이더의 value가 줄어든다

    Player player;
    TextMeshProUGUI timeText;
    float maxLifeTime;

    private void Awake()
    {
        timeText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        player = GameManager.Instance.Player;
        player.onLifeTimeChange += OnLifeTimeChange;
        maxLifeTime = player.maxLifeTime;
        timeText.text = $"{maxLifeTime:f2}Sec";
    }

    private void OnLifeTimeChange(float ratio)
    {
        timeText.text = $"{ratio * maxLifeTime:f2}Sec";
    }
}
