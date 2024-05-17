using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    // OpenButton누를때마다 ResultBard가 열리거나 닫힌다.
    // RestartButton 누를때 마다 기능 실행
    // user나 Enemy가 패배하면 이 창이 열린다
    // 열릴때 승패 여부에 따라 세팅 차이 두기
    // 열릴 때 Analysis에 값 넣기


    ResultBoard resultBoard;
    ResultAnalysis userAnalysis;
    ResultAnalysis enemyAnalysis;

    Button openCloseButton;
    Button restartButton;




    UserPlayer user;
    EnemyPlayer enemy;



    private void Awake()
    {
       

        Transform child = transform.GetChild(0);
        openCloseButton = child.GetComponent<Button>();

        child = transform.GetChild(1);
        resultBoard = child.GetComponent<ResultBoard>();

        child = resultBoard.transform.GetChild(1);
        userAnalysis = child.GetComponent<ResultAnalysis>();

        child = resultBoard.transform.GetChild(2);
        enemyAnalysis = child.GetComponent<ResultAnalysis>();

        child = resultBoard.transform.GetChild(3);
        restartButton = child.GetComponent<Button>();

        openCloseButton.onClick.AddListener(resultBoard.Toggle);
        restartButton.onClick.AddListener(Restart);
    }

    private void Start()
    {
        GameManager gameManager = GameManager.Instance;

        user = gameManager.UserPlayer;
        enemy = gameManager.EnemyPlayer;

        user.onActionEnd += () => 
        {
            userAnalysis.TotalAttackCount = user.TotalAttackCount;
            userAnalysis.SuccessAttackCount = user.SuccessAttackCount;
            userAnalysis.FailAttackCount = user.FailAttackCount;
            userAnalysis.SuccessAttackRate = (float)user.SuccessAttackCount / user.TotalAttackCount;
        };

        enemy.onActionEnd += () =>
        {
            enemyAnalysis.TotalAttackCount = enemy.TotalAttackCount;
            enemyAnalysis.SuccessAttackCount = enemy.SuccessAttackCount;
            enemyAnalysis.FailAttackCount = enemy.FailAttackCount;
            enemyAnalysis.SuccessAttackRate = (float)enemy.SuccessAttackCount / enemy.TotalAttackCount;
        };

        user.onDefeat += () =>  Open(false);
        enemy.onDefeat += () => Open(true);
        Close();


    }



    /// <summary>
    /// ResultPanel여는 함수
    /// </summary>
    void Open(bool player)
    {

        gameObject.SetActive(true);
        resultBoard.SetVictoryDefeat(player);
        
    }

    /// <summary>
    /// ResultPanel닫는 함수
    /// </summary>
    void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 재시작 버튼 누름
    /// </summary>
    private void Restart()
    {
        SceneManager.LoadScene(1);
    }


}
