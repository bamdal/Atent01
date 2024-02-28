using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{

    static float sideDistance = 1.0f;
    static float diagonalDistance = 1.4f;

    /// <summary>
    /// 맵과 시작점과 도착점을 받아서 경로를 계산하는 함수
    /// </summary>
    /// <param name="map">길을 찾을 맵</param>
    /// <param name="start">시작점</param>
    /// <param name="end">도착점</param>
    /// <returns>시작점에서 도착점까지의 경로, 길을 못찾으면 null</returns>
    public static List<Vector2Int> PathFine(GridMap map, Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = null;

        if (map.IsValidPosition(start) && map.IsValidPosition(end) && !map.IsWall(start) && !map.IsWall(end))   // 평지인지 아닌지 확인하면 맵 안인지도 확인 가능
        {
            // start와 end가 벽이아니고 맵안이다.
            map.ClearMapData();

            List<Node> open = new List<Node>();     // open list : 앞으로 탐색할 노드의 리스트
            List<Node> close = new List<Node>();    // close list : 탐색이 완료된 노드의 리스트

            Node current = map.GetNode(start);
            current.G = 0.0f;
            current.H = GetHeuristic(current, end);
            open.Add(current);

            // A* 루프 시작 (핵심 루틴)
            while (open.Count > 0)
            {
                open.Sort();        // f값을 기준으로 정렬
                current = open[0];  // 제일 앞에있는 노드(f값이 가장 작은값)를 currnet로 설정
                open.RemoveAt(0);   // open리스트에서 제일 앞에 있는 노드를 제거

                if (current != end)
                {
                    //목적지가 아니다
                    close.Add(current); // close리스트에 current를 추가해서 탐색이 완료되었음을 표시

                    // current의 주변 8 방향을 open리스트에 추가
                    for (int y = -1; y < 2; y++)
                    {
                        for (int x = -1; x < 2; x++)
                        {
                            Node node = map.GetNode(current.X + x, current.Y + y);   // 주변 노드 가져오기

                            // 스킵할 노드 인지 확인
                            // 벽, 자기자신, 맵밖, close 리스트 , 대각선으로 가는데 옆에 벽이 있는 경우
                            if (node == null) continue;     // 맵밖
                            if (node == current) continue;  // 자기자신
                            if (node.nodeType == Node.NodeType.Wall) continue;  // 벽
                            if (close.Exists((x) => x == node)) continue;   // close리스트의 모든 요소(x)를 node와 비교하고
                                                                            // 하나라도 같으면 true, 전부 다르면 false
                            bool isDiagonal = (x * y) != 0; // 대각선인지 확인 (true면 대각선)
                            if (isDiagonal &&
                                (map.IsWall(current.X + x, current.Y) || map.IsWall(current.X, current.Y + y))
                                ) continue;

                            // current에서 (x,y)로 가는데 걸리는 거리를 확정
                            float distance = isDiagonal ? diagonalDistance : sideDistance;

                            // G값을 갱신할지 결정
                            if (node.G > current.G + distance)
                            {
                                // 원래 가지던 경로보다 currnet를 거쳐서 이동하는것이 더 빠르다 (갱신 필요)
                                if (node.parent == null)    // 부모가 없으면 아직 open리스트에 안들어 갔다고 판단
                                {
                                    node.H = GetHeuristic(node, end);   // 휴리스틱 계산
                                    open.Add(node);                     // open리스트에 추가
                                }

                                node.G = current.G + distance;          // G값 설정
                                node.parent = current;                  // 부모 설정
                            }

                        }
                    }
                }
                else
                {
                    // 목적지에 도착했다.
                    break;  // 목적지에 도착했으면 while 종료
                }

            }

            // 마무리 작업(도착점에 도착 or 길을 못 찾았다)
            if (current == end) // 도착점에 도착
            {
                // 경로 만들기
                path = new List<Vector2Int>();
                Node result = current;
                while (result != null)   // result가 start가 될 때까지 반복
                {
                    path.Add(new Vector2Int(result.X, result.Y));   // 각 current의 위치를 추가
                    result = result.parent;

                }
                path.Reverse(); // 도착점 -> 시작점으로 되어있는 경로 뒤집기
            }

        }

        return path;
    }

    /// <summary>
    /// 휴리스틱 값을 계산하는 함수(현재 위치에서 목적지까지의 예상 거리)
    /// </summary>
    /// <param name="current">현재 노드</param>
    /// <param name="end">도착지점</param>
    /// <returns>예상 거리</returns>
    private static float GetHeuristic(Node current, Vector2Int end)
    {

        return Mathf.Abs(current.X - end.x) + Mathf.Abs(current.Y - end.y);
    }
}
