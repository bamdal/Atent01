using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;


public class GameManager : Singleton<GameManager>
{

    Player player;
    private int clearScore = 100;
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

    public int Addscore1 { get => Addscore; set => Addscore = value; }
    public int ClearScore { get => clearScore; set => clearScore = value; }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        player = FindAnyObjectByType<Player>();
    }

    private void Start()
    {
        
        Player.onScoreChange += ClearGame;

        Addscore1 = 0;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name == "MainMap")
            Player.onScoreChange += ClearGame;
    }


    

    void ClearGame(int score)
    {
        Addscore1 = score;
        Debug.Log($"Addscore{Addscore1}");
        if (ClearScore  <= Addscore1)
        {
            SceneManager.LoadScene("ClearScene");
        
        }

    }
}

