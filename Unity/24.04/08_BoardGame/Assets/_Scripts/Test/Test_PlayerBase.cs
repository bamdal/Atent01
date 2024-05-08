using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Test_PlayerBase : TestBase
{
    public Button reset;
    public Button random;
    public Button resetAndRandom;

    public PlayerBase user;
    public PlayerBase enemy;


    private void Start()
    {
        user = GameManager.Instance.UserPlayer;
        enemy = GameManager.Instance.EnemyPlayer;

        bool isShow = GameManager.Instance.IsTestMode;

        reset.onClick.AddListener(() => { user.Clear(); enemy.Clear();  });
        random.onClick.AddListener(() => { user.AutoShipDeployment(true); enemy.AutoShipDeployment(isShow); });
        resetAndRandom.onClick.AddListener(() => 
        { 
            user.Clear();
            enemy.Clear();
            user.AutoShipDeployment(true);
            enemy.AutoShipDeployment(isShow);
        });
    }

    protected override void OnTestLClick(InputAction.CallbackContext context)
    {
        Vector2Int attackPos = user.Board.GetMouseGridPosition();

        enemy.Attack(attackPos);
        attackPos = enemy.Board.GetMouseGridPosition();
        user.Attack(attackPos);
    }




    protected override void OnTestRClick(InputAction.CallbackContext context)
    {
        // 그 위치에 배치된 배를 배치 해제 유저만
        Vector2Int grid = user.Board.GetMouseGridPosition();
        ShipType type = user.Board.GetShipTypeOnBoard(grid);
        if(type != ShipType.None)
        {
            UserPlayer userPlayer = user as UserPlayer;
            userPlayer.UndoShipDeploy(type);
        }
        
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        user.AutoAttack();
    }
    protected override void OnTest2(InputAction.CallbackContext context)
    {
        enemy.AutoAttack();
    }


}
