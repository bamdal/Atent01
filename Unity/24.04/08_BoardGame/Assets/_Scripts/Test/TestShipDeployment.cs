using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestShipDeployment : TestShipMovement 
{
    // 1~5 함선 선택    ( 선택된 배는 활성화)
    // 보드 좌클릭시 선택한 함선 배치
    // 함선 우클릭시 배치해제    (배치 해제시 비활성화해서 안보이게)


    Ship[] testShips;

    
    Ship TargetShip
    {
        get => ship;
        set
        {
     
            
            if (ship != value) 
            {
                if (value == null && ship != null)
                {
                    Renderer renderer = ship.GetComponentInChildren<Renderer>(true);
                    renderer.material = ShipManager.Instance.NormalShipMaterial;
                }
                ship = value;
                ship?.gameObject.SetActive(true);


            }
        }
    }

    private void Start()
    {

        testShips = new Ship[ShipManager.Instance.ShipTypeCount];
        testShips[(int)ShipType.Carrier -1] = ShipManager.Instance.MakeShip(ShipType.Carrier,transform);
        testShips[(int)ShipType.BattleShip -1] = ShipManager.Instance.MakeShip(ShipType.BattleShip,transform);
        testShips[(int)ShipType.Destroyer -1] = ShipManager.Instance.MakeShip(ShipType.Destroyer,transform);
        testShips[(int)ShipType.Submarine -1] = ShipManager.Instance.MakeShip(ShipType.Submarine,transform);
        testShips[(int)ShipType.PatrolBoat -1] = ShipManager.Instance.MakeShip(ShipType.PatrolBoat,transform);
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        TargetShip = testShips[0];
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        TargetShip = testShips[1];
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        TargetShip = testShips[2];
    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {
        TargetShip = testShips[3];
    }

    protected override void OnTest5(InputAction.CallbackContext context)
    {
        TargetShip = testShips[4];
    }
    protected override void OnTestLClick(InputAction.CallbackContext context)
    {
        Vector2Int grid = board.GetMouseGridPosition();
        if (TargetShip != null && board.ShipDeployment(TargetShip, grid))
        {
            TargetShip = null;
            Debug.Log("배치성공");
        }
        else
        {
            Debug.Log("배치실패");
        }
    }

    protected override void OnTestRClick(InputAction.CallbackContext context)
    {
        Vector2Int grid = board.GetMouseGridPosition();
        ShipType shipType = board.GetShipTypeOnBoard(grid);
        if (shipType != ShipType.None)
        {
            Ship ship = testShips[(int)shipType - 1];
            board.UndoShipDeployment(ship);
        }
    }

    private void Update()
    {
        Vector2Int grid = board.GetMouseGridPosition();
        if (TargetShip != null)
        {
            Renderer renderer = TargetShip.GetComponentInChildren<Renderer>(true);
            if(renderer != null)
                renderer.material=ShipManager.Instance.DeployModeShopMaterial;

            if (board.IsShipDeploymentAvailable(TargetShip, grid))
            {
                ShipManager.Instance.SetDeployModeColor(true);

            }
            else
            {
                ShipManager.Instance.SetDeployModeColor(false);

            }
        }

    }
}
