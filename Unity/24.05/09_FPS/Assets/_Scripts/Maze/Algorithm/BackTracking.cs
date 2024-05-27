using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackTrackingCell : Cell
{
    public bool visited;
    public BackTrackingCell(int x, int y) : base(x, y)
    {
        visited = false;
    }
}
public class BackTracking : Maze
{

 
    protected override void OnSpecificAlgorithmExcute()
    {
        // 재귀적 백트래킹 알고리즘(Recurcive BackTracking Algorithm)
        // 1. 미로에서 랜덤한 지점을 미로에 임시로 추가한다.
        // 2. 마지막에 미로에 추가한 지점에서 갈 수 있는 방향 중 하나를 선택해서 랜덤하게 이동한다.
        // 3. 이동한 곳은 미로에 추가되고, 이전 지점과의 통로가 연결된다
        // 4. 이동 할 곳이 없을 경우 이전 단계의 셀로 돌아간다.
        // 5. 시작지점까지 돌아가면 알고리즘 종료

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                cells[GridToIndex(x, y)] = new BackTrackingCell(x, y);  // 모든 셀 생성(종류에 맞는 cell로 생성)
            }
        }

        int index = Random.Range(0,cells.Length);
        BackTrackingCell start = cells[index] as BackTrackingCell;
        start.visited = true;

        MakeRecursive(start.X, start.Y);

        // 시작지점으로 돌아옴
    }

    /// <summary>
    /// 재귀처리를 위한함수
    /// </summary>
    /// <param name="x">cell의 x위치</param>
    /// <param name="y">cell의 y위치</param>
    void MakeRecursive(int x, int y)
    {
        BackTrackingCell current = cells[GridToIndex(x,y)] as BackTrackingCell;

        Vector2Int[] dirs = { new(0, 1), new(1, 0), new(0, -1), new(-1, 0) };
        Util.Shuffle(dirs); // 랜덤 방향 설정
        
        foreach(Vector2Int dir in dirs)
        {
            Vector2Int newPos  = new(x+dir.x,y+dir.y);
            if(IsInGrid(newPos))    // 미로 안인지 확인
            {
                BackTrackingCell neighbor = cells[GridToIndex(newPos)] as BackTrackingCell;
                if(!neighbor.visited)   // 방문한적이 있는지 확인
                {
                    neighbor.visited = true;    // 방문했다고 표시
                    ConnectPath(current, neighbor); // 두 셀간의 길을 연결
                
                    MakeRecursive(neighbor.X,neighbor.Y);
                    
                }
            }
        }
        // 4방향 체크가 끝남
       
    }
}
