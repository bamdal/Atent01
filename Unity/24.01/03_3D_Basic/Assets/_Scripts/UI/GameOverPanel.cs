using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    TextMeshProUGUI text;
    private void Awake()
    {
        GameManager.Instance.onGameOver += GameOver;
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        GameManager.Instance.Player.onDie += GameOver;
    }

    private void GameOver()
    {
        text.enabled = true;
    }
}
