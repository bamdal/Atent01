using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WayPoints : MonoBehaviour
{

    /// <summary>
    /// 모든 웨이포인트 지점들
    /// </summary>
    public Transform[] waypoints;


    /// <summary>
    /// 현재 이동중인 웨이포인트 지점의 인덱스
    /// </summary>
    int index = 0;

    /// <summary>
    /// 현재 이동중인 웨이포인트 지점의 트랜스폼
    /// </summary>
    public Transform CurrentWayPoint => waypoints[index];

    
    private void Awake()
    {
        waypoints = new Transform[transform.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = transform.GetChild(i);
        }
    }

    /// <summary>
    /// 다음 웨이포인트 지점을 돌려주면서 인덱스를 새로 지정하는 함수
    /// </summary>
    /// <returns>다음 웨이포인트 지점의 트랜스폼</returns>
    public Transform GetNextWaypoint()
    {
        // index를 0 - 1 - 2 - 0 ... 순서로 감
        index++;
        index %= waypoints.Length;
        return waypoints[index];
    }
}
