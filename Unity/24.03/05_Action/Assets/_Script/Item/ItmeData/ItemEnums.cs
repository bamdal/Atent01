using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 코드(ID)
/// </summary>
public enum ItemCode
{
    Ruby = 0,
    Emerald = 1,
    Sapphire = 2

}

/// <summary>
/// 아이템 정렬 기준
/// </summary>
public enum ItemSortBy
{
    Code = 0,   // 코드기준
    Name,   // 이름기준
    Price   // 가격기준
}