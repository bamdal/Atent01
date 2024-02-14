using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameClearPanel : MonoBehaviour
{
    // 게임 클리어 UI

    TextMeshProUGUI text;
    private void Awake()
    {
        GameManager.Instance.onClear += GameClear;
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        
    }

    private void GameClear()
    {
        text.enabled = true;
    }
}
