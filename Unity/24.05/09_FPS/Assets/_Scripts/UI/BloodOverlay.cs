using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodOverlay : MonoBehaviour
{
    public AnimationCurve curve;
    public Color color = Color.red;

    Image image;

    float inverseMaxHP;

    float targetAlpha = 0f;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.color = color;
    }

    private void Start()
    {
        Player player = GameManager.Instance.Player;
        player.onHPChange += OnHPChange;
        inverseMaxHP = 1 / player.MaxHP;
    }

    private void Update()
    {
        color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime);
        image.color = color;
    }

    private void OnHPChange(float hp)
    {
        targetAlpha = curve.Evaluate(1 - hp* inverseMaxHP);
        
    }
}
