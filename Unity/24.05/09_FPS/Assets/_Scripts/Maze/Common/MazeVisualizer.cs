using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeVisualizer : MonoBehaviour
{

    /// <summary>
    /// 셀의 프리펩
    /// </summary>
    public GameObject cellPrefab;

    public GameObject goalPrefab;

    /// <summary>
    /// 마지막으로 비주얼라이저가 그린 미로
    /// </summary>
    Maze maze = null;

    /// <summary>
    /// 이웃의 방향별 그리드 베거를 저장하는 딕셔너리
    /// </summary>
    Dictionary<Direction, Vector2Int> neighborDir;

    /// <summary>
    /// 코너 세트(0 북서,1 북동,2 남동,3 남서)  배열 순서 중요
    /// </summary>
    (Direction, Direction)[] corners = null;

    private void Awake()
    {
        // 이웃의 방향
        neighborDir = new Dictionary<Direction, Vector2Int>(4);
        neighborDir[Direction.North] = new Vector2Int(0, -1);
        neighborDir[Direction.East] = new Vector2Int(1, 0);
        neighborDir[Direction.South] = new Vector2Int(0, 1);
        neighborDir[Direction.West] = new Vector2Int(-1, 0);

        // 코너세트
        corners = new (Direction, Direction)[]
            {
                (Direction.North, Direction.West),
                (Direction.North, Direction.East),
                (Direction.South, Direction.East),
                (Direction.South, Direction.West)
            };
    }

    /// <summary>
    /// 파라메터로 받은 미로를 그리는 함수
    /// </summary>
    /// <param name="maze"></param>
    public void Draw(Maze maze)
    {
        this.maze = maze;
        float size = CellVisualizer.CellSize;



        // 셀의 외형 결정하기
        foreach (var cell in maze.Cells)
        {
            GameObject obj = Instantiate(cellPrefab, transform);
            obj.transform.Translate(cell.X * size,0, -cell.Y * size);
            obj.name = $"Cell_({cell.X},{cell.Y})";

            CellVisualizer cellVisualizer = obj.GetComponent<CellVisualizer>();
            cellVisualizer.RefreshWall(cell.Path);

            // 코너 마감하기
            // - 필요한 코너만 남겨놓기
            // - 모서리쪽 이웃으로 길이 나있으면서 이웃이 가진 모서리 쪽에 벽이 있다.

            int cornerMask = 0;



            for(int i = 0; i<corners.Length; i++)
            {
                if (IsCornerVisible(cell, corners[i].Item1 , corners[i].Item2 ))    // 보여질 코너인지 확인
                {
                    cornerMask |= (1 << i); // 마스크에 기록
                }
            }

            cellVisualizer.RefreshCorner(cornerMask);   // 마스크 데이터를 기반으로 코너 리프레쉬

        }

        GameObject goalObj =  Instantiate(goalPrefab, transform);
        Goal goal = goalObj.GetComponent<Goal>();
        goal.SetRandomPosition(maze.Width,maze.Height);

        Debug.Log("미로 그리기 완료");
    }

    /// <summary>
    /// 모든 셀 제거
    /// </summary>
    public void Clear()
    {
        while (transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            child.SetParent(null);
            Destroy(child.gameObject);
        }

        Debug.Log("미로 지우기 완료");
    }

    /// <summary>
    /// 그리드 좌표를 월드좌료로
    /// </summary>
    /// <param name="x">그리드좌표 x</param>
    /// <param name="y">그리드좌표 y</param>
    /// <returns>월드 좌표</returns>
    public static Vector3 GridToWorld(int x, int y)
    {
        float size = CellVisualizer.CellSize;
        float sizeHalf = size * 0.5f;

        return new(size*x + sizeHalf,0, size*-y - sizeHalf);   
    }

    /// <summary>
    /// 월드좌표를 그리드좌표로
    /// </summary>
    /// <param name="world">월드좌표</param>
    /// <returns>그리드 좌표</returns>
    public static Vector2Int WorldToGrid(Vector3 world)
    {
        float size = CellVisualizer.CellSize;
        
        Vector2Int result = new((int)(world.x / size), -(int)(world.z / size));
        return result;
    }

    /// <summary>
    /// 특정 셀의 코너 중 보여질지 여부를 판단하는 함수
    /// </summary>
    /// <param name="cell">확인할 셀</param>
    /// <param name="dir1">코너를 만드는 방향1</param>
    /// <param name="dir2">코너를 만드는 방향2</param>
    /// <returns>true면 dir1,dir2가 만드는 코너의 부분이 보이게 하고 false면 안보이게 한다</returns>
    bool IsCornerVisible(Cell cell, Direction dir1, Direction dir2)
    {
        bool result = false;
        if (cell.CornerPathCheck(dir1, dir2))                   // dir1과 dir2방향 모두 길이 있다면
        {

            Cell neighborCell1 = maze.GetCell(cell.X + neighborDir[dir1].x, cell.Y + neighborDir[dir1].y);    // dir1 방향의 이웃셀 구하기
            bool dir1Result = neighborCell1.IsWall(dir2);       // 벽이 있는지 여부 확인

            Cell neighborCell2 = maze.GetCell(cell.X + neighborDir[dir2].x, cell.Y + neighborDir[dir2].y);    // dir1 방향의 이웃셀 구하기
            bool dir2Result = neighborCell2.IsWall(dir1);       // 벽이 있는지 여부 확인

            result = (dir1Result && dir2Result);

        }

        return result;
    }
}
