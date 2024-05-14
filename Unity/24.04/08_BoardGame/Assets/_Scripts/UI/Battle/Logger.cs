
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;


public class Logger : MonoBehaviour
{
    public Color userColor = Color.green;
    public Color enemyColor = Color.red;
    public Color shipColor = Color.blue;
    public Color turnColor = Color.yellow;

    string userTextColor;
    string enemyTextColor;
    string shipTextColor;
    string turnTextColor;


    TextMeshProUGUI log;
    const int MaxLineCount = 20;

    List<string> lines;
    StringBuilder builder;

    GameManager gameManager;

    const string YOU = "당신";
    const string ENEMY = "적";

    private void Awake()
    {
        log = GetComponentInChildren<TextMeshProUGUI>();
        lines = new List<string>(MaxLineCount +1);
        builder = new StringBuilder(MaxLineCount +1);

        userTextColor = ColorUtility.ToHtmlStringRGBA(userColor);
        enemyTextColor = ColorUtility.ToHtmlStringRGBA(enemyColor);
        shipTextColor = ColorUtility.ToHtmlStringRGBA(shipColor);
        turnTextColor = ColorUtility.ToHtmlStringRGBA(turnColor);
        log.text = $"<#{turnTextColor}>1</color>턴 시작 \n";
    }


    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.TurnController.onTurnStart += Log_TurnStart;

        UserPlayer user = gameManager.UserPlayer;
        EnemyPlayer enemy = gameManager.EnemyPlayer;



        foreach (var ship in user.Ships)
        {
            ship.onHit += (target) => Log_AttackSuccess(false, target); // 유저의 함선이 공격당했다
            ship.onSink = (target) => { Log_ShipSinking(false, target); } + ship.onSink;    // log가 먼저 나올수 있게 순서 조절
        }

        foreach (var ship in enemy.Ships)
        {
            ship.onHit += (target) => Log_AttackSuccess(true, target);
            ship.onSink = (target) => { Log_ShipSinking(true, target); } + ship.onSink;
        }

        user.onDefeat += () => Log_Defeat(true);
        enemy.onDefeat += () => Log_Defeat(false);

        user.onAttackFail +=  Log_AttackFail;
        enemy.onAttackFail += Log_AttackFail;

        Clear();

        Log_TurnStart(1);

    }

    void Log(string text)
    {
        lines.Add(text);
        if (lines.Count > MaxLineCount)
        {
            lines.RemoveAt(0);
        }

        builder.Clear();
        foreach (var line in lines)
        {
            builder.AppendLine(line);
        }

        log.text = builder.ToString();
    }

    void Clear()
    {
        lines.Clear();
        log.text = string.Empty;
    }

    void Log_TurnStart(int number)
    {
        Log($"<#{turnTextColor}>{number}</color>번째 턴이 시작되었습니다");
    }

    void Log_AttackSuccess(bool isUser, Ship ship)
    {
        string attackerName;
        string attackerColor;
        string hitterName;
        string hitterColor;
        if(isUser)
        {
            attackerName = YOU;
            attackerColor = userTextColor;
            hitterName = ENEMY;
            hitterColor = enemyTextColor;
        }
        else
        {
            attackerName = ENEMY;
            attackerColor = enemyTextColor;
            hitterName = YOU;
            hitterColor = userTextColor;
        }

        Log($"<#{attackerColor}>{attackerName}</color>의 공격\t : <#{hitterColor}>{hitterName}</color>의 <#{shipTextColor}>{ship.ShipName}</color>이 공격 받았다");
    }

    void Log_AttackFail(bool isUser)
    {
        string attackerName;
        string attackerColor;
        if (isUser)
        {
            attackerName = YOU;
            attackerColor = userTextColor;
        }
        else
        {
            attackerName = ENEMY;
            attackerColor = enemyTextColor;
        }

        Log($"<#{attackerColor}>{attackerName}</color>의 공격\t : <#{attackerColor}>{attackerName}</color>의 공격이 빗나갔다");

    }

    void Log_ShipSinking(bool isUser, Ship ship)
    {
        string attackerName;
        string attackerColor;
        string hitterName;
        string hitterColor;
        if (isUser)
        {
            attackerName = YOU;
            attackerColor = userTextColor;
            hitterName = ENEMY;
            hitterColor = enemyTextColor;
        }
        else
        {
            attackerName = ENEMY;
            attackerColor = enemyTextColor;
            hitterName = YOU;
            hitterColor = userTextColor;
        }

        Log($"<#{attackerColor}>{attackerName}</color>의 공격\t : <#{hitterColor}>{hitterName}</color>의 <#{shipTextColor}>{ship.ShipName}</color>이 침몰되었다");

    }

    void Log_Defeat(bool isUser)
    {
        string temp = $"<b><#{userTextColor}>{YOU}</color></b>";

        if (isUser)
        {
            Log($"{temp}의 패배....");
        }
        else
        {
            Log($"{temp}의 승리!");
        }
    }
}
