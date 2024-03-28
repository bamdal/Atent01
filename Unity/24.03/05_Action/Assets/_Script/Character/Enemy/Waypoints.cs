using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 웨이포인트를 저장하고 처리하는 클래스
/// </summary>
public class Waypoints : MonoBehaviour
{

    public Transform Current
    {
        get
        {
            Transform result = waypoint[index];
            NextToIndex();
            Debug.Log(result);
            return result;
        }

    }

    uint index = 0;
    public Transform[] waypoint;

    private void Awake()
    {
        
    }


    void NextToIndex()
    {
        index++;
        index %= (uint)waypoint.Length;
    }
}
