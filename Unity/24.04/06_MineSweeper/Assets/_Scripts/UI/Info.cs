using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Info : MonoBehaviour
{
    TextMeshProUGUI action;
    TextMeshProUGUI find;
    TextMeshProUGUI notFind;

    GameManager gameManager;

    private void Awake()
    {
        Transform child = transform.GetChild(0);
        child = child.GetChild(1);
        action = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(1);
        child = child.GetChild(1);
        find = child.GetComponent<TextMeshProUGUI>();
        child = transform.GetChild(2);
        child = child.GetChild(1);
        notFind = child.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;

        gameManager.onGameReady += OnGameReady;
        gameManager.onActionCountChange += OnActionCountChange;
        gameManager.onGameClear += OnGameEnd;
        gameManager.onGameOver += OnGameEnd;
    }

    /// <summary>
    /// 게임상태가 Ready가 되면 실행되는함수
    /// </summary>
    private void OnGameReady()
    {
        action.text = "???";
        find.text = "???";
        notFind.text = "???";
    }

    /// <summary>
    /// 클리어나 게임오버가 되면 불리는함수
    /// </summary>
    private void OnGameEnd()
    {
        
        find.text = gameManager.Board.FoundMineCount.ToString();
        notFind.text = gameManager.Board.NotFoundMineCount.ToString();
    }

    /// <summary>
    /// 플레이어의 행동회수가 변경되면 실행되는 함수
    /// </summary>
    /// <param name="count">변경된 행동 횟수</param>
    private void OnActionCountChange(int count)
    {
        action.text = count.ToString();
    }
}
