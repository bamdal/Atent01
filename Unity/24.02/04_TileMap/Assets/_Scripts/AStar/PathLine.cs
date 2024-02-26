using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// 경로를 그리는 함수
    /// </summary>
    /// <param name="map">월드 좌표 계산용 맵</param>
    /// <param name="path">그리드 좌표로 구성된 경로</param>
    public void DrawPath(TileGridMap map, List<Vector2Int> path)
    {
        if (map != null && path != null) 
        {
            lineRenderer.positionCount = path.Count;    // 경로 개수만큼 라인랜더러의 위치 추가

            int index = 0;
            foreach (Vector2Int pos in path)            // list 순회
            {
                Vector2 world = map.GridToWorld(pos);   // 리스트에 있는 위치를 월드좌표로 변경
                lineRenderer.SetPosition(index, world); // 라인랜더러에 설정
                index++;                                // 인덱스 증가
            }
        
        }

    }

    public void ClearPath()
    {
        if(lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }
}
