using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Test_BattleResult : TestBase
{
    public ResultPanel panel;
    public ResultBoard board;
    public ResultAnalysis analysis1;
    public ResultAnalysis analysis2;

    UserPlayer user;
    EnemyPlayer enemy;

    private void Start()
    {
        GameManager gameManager = GameManager.Instance;

        user = gameManager.UserPlayer;
        enemy = gameManager.EnemyPlayer;

        user.AutoShipDeployment(true);
        enemy.AutoShipDeployment(true);

        gameManager.GameState = GameState.Battle;
        user.Test_BindInputFuncs();


    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        board.Toggle();
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        board.SetVictoryDefeat(false);
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        board.SetVictoryDefeat(true);
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        GameManager.Instance.UserPlayer.Test_Defeat();
    }

    protected override void OnTest5(InputAction.CallbackContext context)
    {
        GameManager.Instance.EnemyPlayer.Test_Defeat();

    }

}
