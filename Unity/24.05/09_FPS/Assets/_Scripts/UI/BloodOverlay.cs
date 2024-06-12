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

    private void OnHPChange(float hp)
    {
        color.a = curve.Evaluate(1 - hp* inverseMaxHP);
        image.color = color;
    }
}
