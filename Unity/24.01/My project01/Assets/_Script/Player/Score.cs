using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Score : MonoBehaviour
{

    TextMeshProUGUI score;
    Player player;

    /// <summary>
    /// 목표로하는 최종 점수
    /// </summary>
    int goalScore = 0;

    /// <summary>
    /// 현재 보여지는 점수
    /// </summary>
    float currentScore = 0.0f;


    /// <summary>
    /// 점수가 올라가는 속도
    /// </summary>
    public float scoreUpSpeed = 10.0f;

    private void Awake()
    {
        score = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        player = FindAnyObjectByType<Player>();
        player.onScoreChange += RefreshScore;

        goalScore = 0;
        currentScore = 0.0f;

        score.text = $"Collecting 100 coins";
    }

    private void RefreshScore(int newScore)
    {
        goalScore = newScore;

    }

    private void Update()
    {
        if (currentScore < goalScore) // 점수가 올라가는 중
        {
            float speed = Mathf.Max(scoreUpSpeed, (goalScore - currentScore) * 5.0f);
            currentScore += Time.deltaTime * speed;
            currentScore = Mathf.Min(currentScore, goalScore);

            int temp = (int)currentScore;
            score.text = $"Coin : {temp:d5}";

        }
    }


}
