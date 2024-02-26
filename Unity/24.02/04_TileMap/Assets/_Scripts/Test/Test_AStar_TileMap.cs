using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Test_AStar_TileMap : TestBase
{
    public Tilemap background;
    public Tilemap obstacle;


    TileGridMap gridMap;

    public Vector2Int start;
    public Vector2Int end;

    public PathLine PathLine;
    private void Start()
    {
        gridMap = new TileGridMap(background, obstacle);
        PathLine.ClearPath();
    }

    protected override void OnTestLClick(InputAction.CallbackContext _)
    {
        Vector2 screenPosition =  Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        Vector2Int gridPosition = (Vector2Int)background.WorldToCell(worldPosition);
        Debug.Log(gridPosition);
        
        if (!IsWall(gridPosition))
        {
            start = gridPosition;
        }
    }

    protected override void OnTestRClick(InputAction.CallbackContext _)
    {
        // 클릭한 위치에 타일이 있는지 없는지 확인
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        Vector2Int gridPosition = (Vector2Int)obstacle.WorldToCell(worldPosition);

        if (!IsWall(gridPosition))
        {
            end = gridPosition;

            List<Vector2Int> path =AStar.PathFine(gridMap,start, end);
            PathLine.DrawPath(gridMap,path);
        }


    }

    /// <summary>
    /// 지정된 위치에 타일이 있으면 벽, 아니면 빈곳
    /// </summary>
    /// <param name="gridPosition">확인할 위치</param>
    /// <returns>true면 벽, false면 빈곳</returns>
    bool IsWall(Vector2Int gridPosition)
    {

        TileBase tile = obstacle.GetTile((Vector3Int)gridPosition);
        return tile != null ;
  

    }

    public void PrintList(List<Vector2Int> path)
    {
        string str = "";
        foreach (Vector2Int v in path)
        {
            str += $"{v} -> ";
        }
        Debug.Log(str + "End");
    }

    protected override void OnTest1(InputAction.CallbackContext context)
    {
        // Tilemap 그리다 한번 늘어난 최대 사이즈는 줄어들지 않는다
        Debug.Log(background.size.x); // background의 가로에 들어있는 셀의 개수(가로길이)
        Debug.Log(background.size.y); // background의 세로에 들어있는 셀의 개수(가로길이)
        Debug.Log(obstacle.size.x); // background의 가로에 들어있는 셀의 개수(가로길이)

        Debug.Log(obstacle.size.y); // background의 세로에 들어있는 셀의 개수(가로길이)
    }

    protected override void OnTest2(InputAction.CallbackContext context)
    {
        // background.origin : background에 있는 셀중에 왼쪽 아래가 정점 
        Debug.Log(background.origin);
        Debug.Log(obstacle.origin);
    }

    protected override void OnTest3(InputAction.CallbackContext context)
    {
        // cellBounds.min 가장 왼쪽아래 좌표
        // cellBounds.max 가장 오른쪽위 좌표

        Debug.Log($"{background.cellBounds.min},{background.cellBounds.max}");
        Debug.Log($"{obstacle.cellBounds.min},{obstacle.cellBounds.max}");

    }

    protected override void OnTest4(InputAction.CallbackContext context)
    {

    }
}
