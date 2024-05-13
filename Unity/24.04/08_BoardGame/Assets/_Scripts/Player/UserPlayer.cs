using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPlayer : PlayerBase
{

    /// <summary>
    /// 지금 배치하려는 함선
    /// </summary>
    Ship selectedShip;

    /// <summary>
    /// 지금 배치하려는 함선
    /// </summary>
    protected Ship SelectedShip
    {
        get => selectedShip;
        set
        {
            if (selectedShip != null && !selectedShip.IsDeployed)    // 이전배가 있고 배치가 되지않았을 때만
            {
                selectedShip.gameObject.SetActive(false);   // 이전배는 안보이게
            }

            selectedShip = value;
            if (selectedShip != null && !selectedShip.IsDeployed)   // 새로 배가 설정되면
            {

                selectedShip.SetMaterialType(false);    // 머티리얼 배치모드로 바꾸기
                selectedShip.transform.position = Board.GridToWorld(Board.GetMouseGridPosition()) + Board.transform.position;  // 마우스 위치로 옮기고
                SetSelectedShipColor();               // 배치가능한지 머터리얼수정
                selectedShip.gameObject.SetActive(true);// 보여주기
            }
        }
    }

    /// <summary>
    /// 모든 함선이 배치되었는지 확인하는 프로퍼티(true면 모든 함선이배치됨)
    /// </summary>
    public bool IsAllDeployed
    {
        get
        {
            bool result = true;
            foreach (var ship in Ships)
            {
                if(!ship.IsDeployed)
                {
                    result = false; // 함선이 하나라도 배치되지 않았으면 false로 끝내기
                    break;
                }
            }
            return result;
        }
    }

    protected override void Start()
    {
        base.Start();
        //AutoShipDeployment(true);
        opponent = gameManager.EnemyPlayer;

        gameManager.InputController.onMouseClick += OnMouseClick;
        gameManager.InputController.onMouseMove += OnMouseMove;
        gameManager.InputController.onMouseWheel += OnMouseWheel;

    }


    // 함선 배치 및 해재용 함수 -------------------------------------

    public void SelectShipDeploy(ShipType shipType)
    {
        SelectedShip = Ships[(int) shipType-1];
    }

    /// <summary>
    /// 특정 종류의 함선을 배치 해제하는 함수
    /// </summary>
    /// <param name="shipType">배치 취소할 함수</param>
    public void UndoShipDeploy(ShipType shipType)
    {
        Board.UndoShipDeployment(Ships[(int)shipType - 1]); // 배치 취소

    }

    /// <summary>
    /// 배에 움직임이 있었을 때 그 상태가 배치가능한지 여부 파악후 색상 변경
    /// </summary>
    private void SetSelectedShipColor()
    {
        bool isSucess = board.IsShipDeploymentAvailable(SelectedShip, SelectedShip.transform.position); // 배치 가능한지 확인
        ShipManager.Instance.SetDeployModeColor(isSucess);

    }

    // 입력 처리용 함수 ==============================================================================================================

    /// <summary>
    /// 마우스 클립 입력이 있었을 떄 실행될 함수
    /// </summary>
    /// <param name="position">마우스 포인터의 스크린 좌표</param>
    private void OnMouseClick(Vector2 position)
    {
        if (gameManager.GameState == GameState.ShipDeployment)
        {
            // 게임이 한선 배치 모드일 때의 처리
            if(SelectedShip != null)        // 배치할 함선이 있으면 
            {
                if (Board.ShipDeployment(SelectedShip, board.GetMouseGridPosition()))   // 함선 배치 시도
                {
                    Debug.Log($"배치 성공 {SelectedShip.gameObject.name}");
                    SelectedShip = null;    // 성공 하면 배치할 함선 null로 비우기
                }
                else
                {
                    Debug.Log($"배치 불가능한 지역");
                }
            }
            else
            {
                // 배치할 함선이 없으면 클릭 된 지점에 배 취소하기
                Vector2Int grid = board.GetMouseGridPosition();
                ShipType shipType = Board.GetShipTypeOnBoard(grid); // 클릭한 지점에 배가 있는지 체크
                if (shipType != ShipType.None)
                {
                    Ship ship = GetShip(shipType);
                    Board.UndoShipDeployment(ship);
                }
                else
                {
                    Debug.Log($"함선이 없음");
                }
            }
        }
        else if (gameManager.GameState == GameState.Battle)
        {
            // 게임이 전투 상태일 때 처리
            Vector2Int grid = opponent.Board.GetMouseGridPosition();
            Attack(grid);
        }
    }

    /// <summary>
    /// 마우스 커서가 움직일 때 실행되는 함수
    /// </summary>
    /// <param name="position">마우스 커서의 스크린 좌표</param>
    private void OnMouseMove(Vector2 position)
    {
        if (gameManager.GameState == GameState.ShipDeployment)
        {
            if (selectedShip != null)
            {
                // 배치 하려는 함선이 있으면
                Vector2Int grid = board.GetMouseGridPosition();
                Vector3 world = board.GridToWorld(grid);
                selectedShip.transform.position = world + board.transform.position;

                SetSelectedShipColor();


            }


        }
    }

    /// <summary>
    /// 마우스 휠 입력이 있을 때 실행되는 함수
    /// </summary>
    /// <param name="delta">휠이움직인 정도</param>
    private void OnMouseWheel(float delta)
    {
        if (gameManager.GameState == GameState.ShipDeployment)
        {
            if (selectedShip != null)
            {

                selectedShip.Rotate(delta < 0);
                SetSelectedShipColor();

            }
        }
    }

#if UNITY_EDITOR
    public void Test_BindInputFuncs()
    {
        gameManager.InputController.onMouseClick += OnMouseClick;
        gameManager.InputController.onMouseMove += OnMouseMove;
        gameManager.InputController.onMouseWheel += OnMouseWheel;
    }

#endif
}
