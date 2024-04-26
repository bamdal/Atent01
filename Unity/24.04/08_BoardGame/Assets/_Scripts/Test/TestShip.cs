using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestShip : TestBase
{
    public Board board;

    public ShipType shipType = ShipType.None;

    private void Start()
    {
        board = FindAnyObjectByType<Board>();
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        ShipManager.Instance.MakeShip(shipType, transform);
    }

    protected override void OnTestLClick(InputAction.CallbackContext context)
    {

        board.GridToWorld(board.GetMouseGridPosition());
    }
}
