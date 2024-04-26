using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Board : MonoBehaviour
{
    // 클릭되어야함
    // 각종 위치관련 유틸리티 함수
    public const int BoardSize = 10;

    /// <summary>
    /// 인덱스 값 -> 그리드 좌표
    /// </summary>
    /// <param name="index">인덱스 값</param>
    /// <returns>보드 상의 위치</returns>
    public Vector2Int IndexToGrid(uint index)
    {
        return new Vector2Int((int)index % BoardSize, (int)index / BoardSize);
    }

    /// <summary>
    /// 인덱스 -> 월드
    /// </summary>
    /// <param name="index">인덱스 값</param>
    /// <returns>월드 좌표</returns>
    public Vector3 IndexToWorld(uint index)
    {
        return Vector3.zero;
    }

    /// <summary>
    /// 그리드 -> 인덱스 값
    /// </summary>
    /// <param name="x">그리드x 값</param>
    /// <param name="y">그리드y 값</param>
    /// <returns>그리드가 보드 안이면 인덱스 값, 아니면 null</returns>
    public int? GridToIndex(int x, int y)
    {
        int? result = null;
        if (IsInBoard(x, y))
        {
            result = y * BoardSize + x;
        }
        return result;
    }


    /// <summary>
    /// 그리드 -> 인덱스 값
    /// </summary>
    /// <param name="grid">그리드 값</param>
    /// <returns>그리드가 보드 안이면 인덱스 값, 아니면 null</returns>
    public int? GridToIndex(Vector2Int grid)
    {
        return grid.y * BoardSize + grid.x;
    }

    /// <summary>
    /// 그리드 -> 월드 좌표
    /// </summary>
    /// <param name="x">x 값</param>
    /// <param name="y">y 값</param>
    /// <returns>월드좌표(그리드 중심점)</returns>
    public Vector3 GridToWorld(int x,int y)
    {
        return new Vector3(x + 0.5f, 0, y - 0.5f);
    }

    /// <summary>
    /// 그리드 -> 월드 좌표
    /// </summary>
    /// <param name="grid">그리드 값</param>
    /// <returns>월드좌표(그리드 중심점)</returns>
    public Vector3 GridToWorld(Vector2Int grid)
    {
        return new Vector3(grid.x + 0.5f, 0, grid.y - 0.5f);
    }

    /// <summary>
    /// 월드좌표 -> 그리드
    /// </summary>
    /// <param name="world">월드 좌표</param>
    /// <returns>그리드</returns>
    public Vector2Int WorldToGrid(Vector3 world)
    {

            Vector2 diff = new(world.x - transform.position.x, transform.position.z - world.z);

            return new Vector2Int(Mathf.FloorToInt(diff.x), Mathf.FloorToInt(diff.y));

    }

    /// <summary>
    /// 입력된 좌표가 보드 안인지 아닌지 확인
    /// </summary>
    /// <param name="x">월드 좌표</param>
    /// <param name="y">월드 좌표</param>
    /// <returns>true면 안 false면 밖</returns>
    public bool IsInBoard(int x, int y)
    {
        Vector2 diff = new(x - transform.position.x, transform.position.z - y);
        if (diff.x < BoardSize && diff.y < BoardSize && diff.x > 0 && diff.y > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 입력된 좌표가 보드 안인지 아닌지 확인
    /// </summary>
    /// <param name="world">월드 좌표</param>
    /// <returns>true면 안 false면 밖</returns>
    public bool IsInBoard(Vector3 world)
    {
        Vector2 diff = new(world.x - transform.position.x, transform.position.z - world.z);
        if (diff.x < BoardSize && diff.y < BoardSize && diff.x > 0 && diff.y > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 입력된 좌표가 보드 안인지 아닌지 확인
    /// </summary>
    /// <param name="world">그리드 좌표</param>
    /// <returns>true면 안 false면 밖</returns>
    public bool IsInBoard(Vector2Int grid)
    {

            return grid.x < BoardSize && grid.y < BoardSize && grid.x > -1 && grid.y > -1;
    
    }



    /// <summary>
    /// 마우스커서 위치의 그리드 좌표
    /// </summary>
    /// <returns>그리드 좌표</returns>
    public Vector2Int GetMouseGridPosition()
    {
        Vector2 screen = Mouse.current.position.ReadValue();
        Vector3 world = Camera.main.ScreenToWorldPoint(screen);
        return WorldToGrid(world);

        //return WorldToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }


}
