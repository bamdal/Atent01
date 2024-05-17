using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDeployData
{

    /// <summary>
    /// 배치된 방향
    /// </summary>
    ShipDirection direction;

    public ShipDirection Direction => direction;

    /// <summary>
    /// 배치된 위치
    /// </summary>
    Vector2Int position;

    public Vector2Int Position => position;

    public ShipDeployData(ShipDirection direction, Vector2Int position)
    {
        this.direction = direction;
        this.position = position;
    }
}
