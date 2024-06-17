using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    Player player;

    private void Start()
    {
        player = GameManager.Instance.Player;
    }
    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameClear();
        }
    }

    public void SetRandomPosition(int width, int height)
    {
        // 랜덤하게 가장자리 Grid 위치 구하기
        Vector2Int result = new Vector2Int();

        int dir = UnityEngine.Random.Range(0, 4);   // 0:북, 1:동, 2:남, 3:서
        switch (dir)
        {
            case 0:
                result.x = UnityEngine.Random.Range(0, width);
                result.y = 0;
                break;
            case 1:
                result.x = width - 1;
                result.y = UnityEngine.Random.Range(0, height);
                break;
            case 2:
                result.x = UnityEngine.Random.Range(0, width);
                result.y = height - 1;
                break;
            case 3:
                result.x = 0;
                result.y = UnityEngine.Random.Range(0, height);
                break;
        }

        // 랜덤하게 가장자리 Grid 위치 구하기
        transform.position = MazeVisualizer.GridToWorld(result.x, result.y);
    }

#if UNITY_EDITOR
    public Vector2Int TestSetRandomPosition(int width, int height)
    {
        Vector2Int result = new Vector2Int();

        int dir = UnityEngine.Random.Range(0, 4); // 북동남서

        switch (dir) 
        {
            case 0:
                result.x = UnityEngine.Random.Range(0,width);
                result.y = 0;
                break;
            case 1:
                result.x = width-1;
                result.y = UnityEngine.Random.Range(0, height);
                break;
            case 2:
                result.x = UnityEngine.Random.Range(0, width);
                result.y = height-1;
                break; 
            case 3:
                result.x = 0;
                result.y = UnityEngine.Random.Range(0, height);
                break;
        }

        return result;
    }
#endif
}
