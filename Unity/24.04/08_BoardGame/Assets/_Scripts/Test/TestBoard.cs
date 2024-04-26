using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestBoard : TestBase
{
    public Board board;

    private void Start()
    {
        board = FindAnyObjectByType<Board>();
    }


    protected override void OnTestLClick(InputAction.CallbackContext context)
    {
        // 디버그로 그리드 좌표 출력
        // 좌표가 안인지 밖인지 판단
        // 찍은 그리드의 중심점 출력

        Vector2Int grid = board.GetMouseGridPosition();
        Debug.Log($"Grid : {grid.x},{grid.y}");

        if (board.IsInBoard(grid))
        {
            Debug.Log("보드 안쪽");
        }
        else
        {
            Debug.Log("보드 바깥쪽");
        }
        Vector3 world = board.GridToWorld(grid);
        Debug.Log($"World : {world}");
    }
}
