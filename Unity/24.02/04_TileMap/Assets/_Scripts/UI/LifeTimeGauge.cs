using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeTimeGauge : MonoBehaviour
{
    // 플레이어 수명에 따라 슬라이더의 value가 줄어든다

    Player player;
    Slider slider;
    Image fill;

    public Color startColor = Color.white;
    public Color endColor = Color.red;
    public Gradient color;
    public AnimationCurve AnimationCurve;

    private void Awake()
    {
        slider= GetComponent<Slider>();
        slider.value = 1;

        Transform child = transform.GetChild(1);
        fill = child.GetChild(0).GetComponent<Image>();
    }

    private void Start()
    {
        player = GameManager.Instance.Player;
        player.onLifeTimeChange += OnLifeTimeChange;
    }

    private void OnLifeTimeChange(float ratio)
    {
        slider.value = ratio;

        //fill.color = Color.Lerp(startColor, endColor, ratio);
        fill.color = color.Evaluate(ratio);
    }
}
