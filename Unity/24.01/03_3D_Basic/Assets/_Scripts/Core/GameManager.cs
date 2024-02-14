using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    Player player;

    public Player Player
    {
        get
        {
            if (player == null)
            {
                OnInitialize();
            }
            return player;
        }
    }

    VirtualStick stick;

    public VirtualStick Stick
    {
        get
        {
            if (stick == null)
            {
                stick = FindAnyObjectByType<VirtualStick>();
            }
            return stick;
        }

    }

    VirtualButton jumpButton;
    public VirtualButton JumpButton
    {
        get
        {
            if (jumpButton == null)
            {
                jumpButton = FindAnyObjectByType<VirtualButton>();
            }
            return jumpButton;
        }
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        player = FindAnyObjectByType<Player>();
    }

    bool isClear = false;

    public Action onClear;

    public void GameClear()
    {
        if (!isClear)
        {
            onClear?.Invoke();
            isClear = true;
        }
    }
       
    bool isGameOver = false;

    public Action onGameOver;

    public void GameOver()
    {
        if (!isGameOver)
        {
            onGameOver?.Invoke();
            isGameOver = true;
        }
    }

}
