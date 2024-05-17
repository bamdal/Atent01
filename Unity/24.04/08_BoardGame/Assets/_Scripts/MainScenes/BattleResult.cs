using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResult : MonoBehaviour
{
    void Start()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.GameState = GameState.Battle;
        CinemachineVirtualCamera vcam = gameManager.GetComponentInChildren<CinemachineVirtualCamera>();
        vcam.m_Lens.OrthographicSize = 10.0f;

        gameManager.UserPlayer.BindInputFuncs();
        // 저장된것 로딩후 실패하면 자동배치)
        if (!gameManager.LoadeShipDeployData())
        {
            gameManager.UserPlayer.AutoShipDeployment(true);
        }
        gameManager.EnemyPlayer.AutoShipDeployment(false);
    }


}
