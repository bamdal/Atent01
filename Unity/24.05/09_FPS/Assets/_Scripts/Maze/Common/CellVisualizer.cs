using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CellVisualizer : MonoBehaviour
{
    /// <summary>
    /// 셀하나의 크기
    /// </summary>
    public const float CellSize = 10.0f;

    GameObject[] walls;

    private void Awake()
    {
        Transform child = transform.GetChild(1);
        walls = new GameObject[child.childCount];
        for(int i = 0; i < walls.Length; i++)
        {
            walls[i] = child.GetChild(i).gameObject;
        }
    }

    /// <summary>
    /// 입력받은 데이터에 맞게 벽의 활성화 여부 재설정
    /// </summary>
    /// <param name="data"></param>
    public void RefreshWall(byte data)
    {
        for(int i =0; i < walls.Length; i++)
        {
            int mask = 1 << i;
            walls[i].SetActive(!((data & mask) != 0));  // 순서대로 마스크 생성한 후 & 연산으로 결과 확인
        } 

    }

    /// <summary>
    /// 현제 셀의 상태를 확인해서 벽 활성화 정보를 리턴하는 함수
    /// </summary>
    /// <returns>북동남서 순서로 1길 0벽</returns>
    public Direction GetPath()
    {
        byte mask = 0;
        for (int i = 0; i < walls.Length; i++)
        {
            if (!walls[i].activeSelf)
            {
                // wall이 active이면 해당 위치의 비트를 1로 설정
                mask |= (byte)(1 << i);
            }
           
        }

        return (Direction)mask;
    }
}
