using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestShipMovement : TestBase
{
    public Board board;


    public Ship ship;

    public ShipType shipType = ShipType.None;
    // ship을 그리드 단위로 움직이기
    // 휠로 ship에 Rotate쓰기

    protected override void OnEnable()
    {
        base.OnEnable();
        inputActions.Test.MouseMove.performed += OnMouseMove;
        inputActions.Test.MouseWheel.performed += OnMouseWheel;

    }


    protected override void OnDisable()
    {
        inputActions.Test.MouseWheel.performed -= OnMouseWheel;
        inputActions.Test.MouseMove.performed -= OnMouseMove;
        base.OnDisable();
    }

    private void Start()
    {
        board = FindAnyObjectByType<Board>();
        ship.Initialize(ShipType.Carrier);
    }

    private void OnMouseMove(InputAction.CallbackContext context)
    {
        if (ship != null)
        {
            Vector2Int grid = board.GetMouseGridPosition();
            Vector3 world = board.GridToWorld(grid);
            ship.transform.position = world + board.transform.position;
            ship.gameObject.SetActive(true);
        }

    }
    private void OnMouseWheel(InputAction.CallbackContext context)
    {
        if (ship != null)
        {
            float wheel = context.ReadValue<float>();
            if (wheel > 0)
            {
                ship.Rotate(false);
            }
            else
            {
                ship.Rotate(true);
            }
        }

    }

}
