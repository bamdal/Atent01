using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeVisualizer : MonoBehaviour
{

    /// <summary>
    /// 셀의 프리펩
    /// </summary>
    public GameObject cellPrefab;

    /// <summary>
    /// 파라메터로 받은 미로를 그리는 함수
    /// </summary>
    /// <param name="maze"></param>
    public void Draw(Maze maze)
    {
        float size = CellVisualizer.CellSize;
        foreach (var cell in maze.Cells)
        {
            GameObject obj = Instantiate(cellPrefab, transform);
            obj.transform.Translate(cell.X * size,0, -cell.Y * size);
            obj.name = $"Cell_({cell.X},{cell.Y})";

            CellVisualizer cellVisualizer = obj.GetComponent<CellVisualizer>();
            cellVisualizer.RefreshWall(cell.Path);
        }
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
    }

    /// <summary>
    /// 그리드 좌표를 월드좌료로
    /// </summary>
    /// <param name="x">그리드좌표 x</param>
    /// <param name="y">그리드좌표 y</param>
    /// <returns>월드 좌표</returns>
    public Vector3 GridToWorld(int x, int y)
    {
        float size = CellVisualizer.CellSize;
        float sizeHalf = size * 0.5f;

        return new(size*x + sizeHalf, size*-y - sizeHalf);   
    }
}
