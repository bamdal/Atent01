using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestShipDeployment : TestBase
{
    public Board board;

    public GameObject ship;

    public ShipType shipType = ShipType.None;
    // ship을 그리드 단위로 움직이기
    // 휠로 ship에 Rotate쓰기

    private void Start()
    {
        board = FindAnyObjectByType<Board>();
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
       
    }

    protected override void OnTestLClick(InputAction.CallbackContext context)
    {
        board.GridToWorld(board.GetMouseGridPosition());
        Ship ship = ShipManager.Instance.MakeShip(shipType,board.transform);
        ship.transform.position = board.GridToWorld(board.GetMouseGridPosition()) ; 
        ship.gameObject.SetActive(true);
    }
}
