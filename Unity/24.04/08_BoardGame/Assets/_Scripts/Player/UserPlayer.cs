using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPlayer : PlayerBase
{


    protected override void Start()
    {
        base.Start();
        AutoShipDeployment(true);
        opponent = gameManager.EnemyPlayer;
    }

    // 함선 배치 및 해재용 함수 -------------------------------------

    /// <summary>
    /// 특정 종류의 함선을 배치 해제하는 함수
    /// </summary>
    /// <param name="shipType">배치 취소할 함수</param>
    public void UndoShipDeploy(ShipType shipType)
    {
        Board.UndoShipDeployment(Ships[(int)shipType - 1]); // 배치 취소
        
    }
}
