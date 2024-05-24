using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    /// <summary>
    /// 미로의 가로길이
    /// </summary>
    protected int width;

    /// <summary>
    /// 미로의 가로길이
    /// </summary>
    public int Width => width;

    /// <summary>
    /// 미로의 세로길이
    /// </summary>
    protected int height;

    /// <summary>
    /// 미로의 세로길이
    /// </summary>
    public int Height => height;


    protected Cell[] cells;
    public Cell[] Cells => cells;

    /// <summary>
    /// 미로를 생성하는 함수
    /// </summary>
    /// <param name="width">미로의 가로 길이</param>
    /// <param name="height">미로의 세로 길이</param>
    /// <param name="seed">랜덤시드값, -1이아니면 지정된 싣 사용</param>
    /// <returns></returns>
    public void MakeMaze(int width, int height, int seed = -1)
    {
        this.width = width;
        this.height = height;

        if(seed != -1)
        {
            Random.InitState(seed);
        }

        cells = new Cell[width * height];

        OnSpecificAlgorithmExcute(); // 각 알고리즘별 코드 실행

       
    }

    /// <summary>
    /// 각 알고리즘별 override해야하는 함수, 미로생성 알고리즘 실행
    /// </summary>
    protected virtual void OnSpecificAlgorithmExcute()
    {
        // cells 안을 생성하고 알고리즘 결과에 맞게 세팅
    }

    /// <summary>
    /// 두 셀사이의 벽을 없애는 함수
    /// </summary>
    /// <param name="from">시작 셀</param>
    /// <param name="to">도착 셀</param>
    protected void ConnectPath(Cell from, Cell to)
    {
        
    }

    protected bool IsInGrid(int x, int y)
    {
     
        
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    protected bool IsInGrid(Vector2Int grid)
    {
        
        return IsInGrid(grid.x,grid.y);
    }

    protected Vector2Int IndexToGrid(int index)
    {
        return new Vector2Int(index%width,index/height);
    }

    protected int GridToIndex(int x, int y) 
    {
        if(IsInGrid(x, y))
            return x + y*height;
        return -1;
    }

    protected int GridToIndex(Vector2Int grid)
    {
        return GridToIndex(grid.x,grid.y);
    }
}