using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


public class GameManager : Singleton<GameManager>
{

    Player player;
    public int clearScore = 100;
    int Addscore=0;
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


    protected override void OnInitialize()
    {
        base.OnInitialize();
        player = FindAnyObjectByType<Player>();
    }

    private void Start()
    {
        
        Player.onScoreChange += ClearGame;
        Player.onDie += GameOver;
        Addscore = 0;
    }

    private void GameOver()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    void ClearGame(int score)
    {
        Addscore = score;
        Debug.Log($"Addscore{Addscore}");
        if (clearScore  < Addscore)
        {
            SceneManager.LoadScene("ClearScene");
        
        }

    }
}

