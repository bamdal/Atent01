using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestShipDeployment : TestShipMovement 
{
    // 1~5 함선 선택    ( 선택된 배는 활성화)
    // 보드 좌클릭시 선택한 함선 배치
    // 함선 우클릭시 배치해제    (배치 해제시 비활성화해서 안보이게)


    protected Ship[] testShips;

    /// <summary>
    /// 현재 확인하고 있는 함선
    /// </summary>
    Ship TargetShip
    {
        get => ship;
        set
        {



            if(ship != null && !ship.IsDeployed)    // 이전배가 있고 배치가 되지않았을 때만
            {
                ship.gameObject.SetActive(false);   // 이전배는 안보이게
            }

            ship = value;
            if (ship != null && !ship.IsDeployed)   // 새로 배가 설정되면
            {

                ship.SetMaterialType(false);    // 머티리얼 배치모드로 바꾸기
                OnShipMovement();               // 배치가능한지 머터리얼수정
                ship.transform.position = board.GridToWorld(board.GetMouseGridPosition()) + board.transform.position;  // 마우스 위치로 옮기고
                ship.gameObject.SetActive(true);// 보여주기
            }







        }
    }

    protected virtual void Start()
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
        // 배치용으로 선택된 배가 있으면 배를 배치 시도
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
        if (shipType != ShipType.None)  // 우클릭 된 지점에 배가 있으면
        {
            Ship ship = testShips[(int)shipType - 1];
            board.UndoShipDeployment(ship); // 배치 취소
        }
    }

    /// <summary>
    /// 배에 움직임이 있었을 때 그 상태가 배치가능한지 여부 파악후 색상 변경
    /// </summary>
    protected override void OnShipMovement()
    {
        bool isSucess = board.IsShipDeploymentAvailable(TargetShip, TargetShip.transform.position); // 배치 가능한지 확인
        ShipManager.Instance.SetDeployModeColor(isSucess);

    }


}
