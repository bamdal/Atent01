using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.VolumeComponent;


public class Board : MonoBehaviour
{
    /// <summary>
    /// 보드크기
    /// </summary>
    public const int BoardSize = 10;

    // 함선 배치관련 함수들 --------------------------------------------------------

    /// <summary>
    /// 보드에 배치되어있는 배의 정도 (빈곳은 None)
    /// </summary>
    ShipType[] shipInfos;


    private void Awake()
    {
        shipInfos = new ShipType[BoardSize * BoardSize];    // 기본적으로 전부 None
    }

    /// <summary>
    /// 함선을 배치하는 함수
    /// </summary>
    /// <param name="ship">배치할 함선(위치, 방향 , 크기 등의 정보 사용)</param>
    /// <param name="grid">배치될 그리드 좌표</param>
    /// <returns>배치 성공시 true, 아니면 false</returns>
    public bool ShipDeployment(Ship ship, Vector2Int grid)
    {
        bool result = IsShipDeploymentAvailable(ship, grid, out Vector2Int[] gridPositions);    // 배치가 가능한 위치인지 확인

        if (result)
        {
            foreach (var pos in gridPositions)
            {
                shipInfos[GridToIndex(pos).Value] = ship.Type;  // 함선 기록
            }

           Vector3 world = GridToWorld(grid);
           ship.transform.position = transform.position + world;            // 함선위치를 옮기고
           ship.Deploy(gridPositions);                 // 함선 개별 처리 수행
        }

        return result;

    }

    /// <summary>
    /// 함선을 배치하는 함수
    /// </summary>
    /// <param name="ship">배치할 함선(위치, 방향 , 크기 등의 정보 사용)</param>
    /// <param name="world">배치될 월드 좌표</param>
    /// <returns>배치 성공시 true, 아니면 false</returns>
    public bool ShipDeployment(Ship ship, Vector3 world)
    {

        return ShipDeployment(ship, WorldToGrid(world));
    }

    /// <summary>
    /// 함선이 특정 위치에 배치될 수 있는지 확인하는 함수
    /// </summary>
    /// <param name="ship">확인할 배</param>
    /// <param name="grid">확인할 배치 좌표(함선의 머리의 그리드 좌표)</param>
    /// <param name="resultPositions">배치가 가능할 때 배치될 위치들(그리드좌표)</param>
    /// <returns>true면 배치가능, false면 배치 불가능</returns>
    public bool IsShipDeploymentAvailable(Ship ship, Vector2Int grid, out Vector2Int[] resultPositions)
    {
        Vector2Int dir = Vector2Int.zero;   // 그리드 위치 계산을 위한 방향 벡터         
        switch (ship.Direction)             // 바라보는 방향에 따라 방향벡터 값 결정
        {
            case ShipDirection.North:
                dir = Vector2Int.up;
                break;
            case ShipDirection.East:
                dir = Vector2Int.left;
                break;
            case ShipDirection.South:
                dir = Vector2Int.down;
                break;
            case ShipDirection.West:
                dir = Vector2Int.right;
                break;
            default:
                break;
        }

        resultPositions = new Vector2Int[ship.Size];    // 확인할 위치 만들기 (배치가능한 경우의 배치 위치)
        for (int i = 0; i < ship.Size; i++)
        {
            resultPositions[i] = grid + dir * i;
        }
        bool result = true;
        foreach (Vector2Int pos in resultPositions)     // 확인할 위치들을 하나씩 전부 확인
        {
            if (!IsInBoard(pos) || IsShipDeployedPosition(pos)) // 보드밖이거나 배가 배치되어있으면
            {
                result = false; // 배치 불가능
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// 함선이 특정 위치에 배치될 수 있는지 확인하는 함수
    /// </summary>
    /// <param name="ship">확인할 배</param>
    /// <param name="grid">확인할 배치 좌표(함선의 머리의 그리드 좌표)</param>
    /// <returns>true면 배치가능, false면 배치 불가능</returns>
    public bool IsShipDeploymentAvailable(Ship ship, Vector2Int grid)
    {
        return IsShipDeploymentAvailable(ship, grid, out _);
    }




    /// <summary>
    /// 함선이 특정 위치에 배치될 수 있는지 확인하는 함수
    /// </summary>
    /// <param name="ship">확인할 배</param>
    /// <param name="world">확인할 배치 좌표(함선의 머리의 월드 좌표)</param>
    /// <returns>true면 배치가능, false면 배치 불가능</returns>
    public bool IsShipDeploymentAvailable(Ship ship, Vector3 world)
    {
        return IsShipDeploymentAvailable(ship, WorldToGrid(world));
    }

    /// <summary>
    /// 보드의 특정 위치에 있는 배의 종류를 확인하느 함수
    /// </summary>
    /// <param name="grid">확인할 위치</param>
    /// <returns>배가 없거나 보드밖이면 None, 배가 있으면 배의 종류</returns>
    public ShipType GetShipTypeOnBoard(Vector2Int grid)
    {
        ShipType result = ShipType.None;

        int? index = GridToIndex(grid); // 보드안이고 그리드값이 보드 크기 안일 때 값이 나옴
        if (index != null)
            result = shipInfos[GridToIndex(grid).Value];    // 제대로 된 값일 때 배의 정보 리턴


        return result;
    }

    public ShipType GetShipTypeOnBoard(Vector3 world)
    {
        return GetShipTypeOnBoard(WorldToGrid(world));
    }

    /// <summary>
    /// 함선 배치 해제 함수
    /// </summary>
    /// <param name="ship">배치 해제할 함선</param>
    public void UndoShipDeployment(Ship ship)
    {
        if (ship.IsDeployed)
        {
            foreach (var pos in ship.Positions)
            {
                shipInfos[GridToIndex(pos).Value] = ShipType.None;
            }
            ship.UnDeploy();
            ship.gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// 보드 초기화용 함수
    /// </summary>
    /// <param name="ships"></param>
    public void ResetBoard(Ship[] ships)
    {
        // ships 전부 배치 해제
        foreach (var ship in ships)
        {
            if (ship.IsDeployed)
            {
                UndoShipDeployment(ship);
            }
        }
    }

    // 좌표변환 유틸리티 -------------------------------------

    /// <summary>
    /// 인덱스 값 -> 그리드 좌표
    /// </summary>
    /// <param name="index">인덱스 값</param>
    /// <returns>보드 상의 위치</returns>
    public Vector2Int IndexToGrid(uint index)
    {
        return new Vector2Int((int)index % BoardSize, (int)index / BoardSize);
    }

    /// <summary>
    /// 인덱스 -> 월드
    /// </summary>
    /// <param name="index">인덱스 값</param>
    /// <returns>월드 좌표</returns>
    public Vector3 IndexToWorld(uint index)
    {
        return GridToWorld(IndexToGrid(index));
    }

    /// <summary>
    /// 그리드 -> 인덱스 값
    /// </summary>
    /// <param name="x">그리드x 값</param>
    /// <param name="y">그리드y 값</param>
    /// <returns>그리드가 보드 안이면 인덱스 값, 아니면 null</returns>
    public int? GridToIndex(int x, int y)
    {
        int? result = null;
        if (IsInBoard(x, y))
        {
            result = y * BoardSize + x;
        }
        return result;
    }


    /// <summary>
    /// 그리드 -> 인덱스 값
    /// </summary>
    /// <param name="grid">그리드 값</param>
    /// <returns>그리드가 보드 안이면 인덱스 값, 아니면 null</returns>
    public int? GridToIndex(Vector2Int grid)
    {
        return grid.y * BoardSize + grid.x;
    }

    /// <summary>
    /// 그리드 -> 월드 좌표
    /// </summary>
    /// <param name="x">x 값</param>
    /// <param name="y">y 값</param>
    /// <returns>월드좌표(그리드 중심점)</returns>
    public Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(x + 0.5f, 0, y - 0.5f);
    }

    /// <summary>
    /// 그리드 -> 월드 좌표
    /// </summary>
    /// <param name="grid">그리드 값</param>
    /// <returns>월드좌표(그리드 중심점)</returns>
    public Vector3 GridToWorld(Vector2Int grid)
    {
        return new Vector3(grid.x + 0.5f, 0, -grid.y - 0.5f);
    }

    /// <summary>
    /// 월드좌표 -> 그리드
    /// </summary>
    /// <param name="world">월드 좌표</param>
    /// <returns>그리드</returns>
    public Vector2Int WorldToGrid(Vector3 world)
    {

        Vector2 diff = new(world.x - transform.position.x, transform.position.z - world.z);

        return new Vector2Int(Mathf.FloorToInt(diff.x), Mathf.FloorToInt(diff.y));

    }

    /// <summary>
    /// 입력된 좌표가 보드 안인지 아닌지 확인
    /// </summary>
    /// <param name="x">월드 좌표</param>
    /// <param name="y">월드 좌표</param>
    /// <returns>true면 안 false면 밖</returns>
    public bool IsInBoard(int x, int y)
    {
        Vector2 diff = new(x - transform.position.x, transform.position.z - y);
        if (diff.x < BoardSize && diff.y < BoardSize && diff.x > 0 && diff.y > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 입력된 좌표가 보드 안인지 아닌지 확인
    /// </summary>
    /// <param name="world">월드 좌표</param>
    /// <returns>true면 안 false면 밖</returns>
    public bool IsInBoard(Vector3 world)
    {
        Vector2 diff = new(world.x - transform.position.x, transform.position.z - world.z);
        if (diff.x < BoardSize && diff.y < BoardSize && diff.x > 0 && diff.y > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 입력된 좌표가 보드 안인지 아닌지 확인
    /// </summary>
    /// <param name="world">그리드 좌표</param>
    /// <returns>true면 안 false면 밖</returns>
    public bool IsInBoard(Vector2Int grid)
    {

        return grid.x < BoardSize && grid.y < BoardSize && grid.x > -1 && grid.y > -1;

    }

    /// <summary>
    /// 특정 위치에 배가 배치되어있는지 확인하는 함수
    /// </summary>
    /// <param name="grid">확인할 위치(그리드좌표)</param>
    /// <returns>true면 있고 false면 없다</returns>
    bool IsShipDeployedPosition(Vector2Int grid)
    {
        int? index = GridToIndex(grid);
        bool result;
        if (index.HasValue)
        {
            result = shipInfos[GridToIndex(grid).Value] != ShipType.None;   // index가 값이 있으면 shipInfo 확인

        }
        else
        {
            result = false;

        }

        return result;
    }


    /// <summary>
    /// 마우스커서 위치의 그리드 좌표
    /// </summary>
    /// <returns>그리드 좌표</returns>
    public Vector2Int GetMouseGridPosition()
    {
        Vector2 screen = Mouse.current.position.ReadValue();
        Vector3 world = Camera.main.ScreenToWorldPoint(screen);
        return WorldToGrid(world);

        //return WorldToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }


}
