using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 코드(ID)
/// </summary>
public enum ItemCode : byte // 256 -> 2^8 개수 제한
{
    Ruby = 0,
    Emerald = 1,
    Sapphire = 2,
    CopperCoin = 3,
    SilverCoin = 4,
    GoldCoin = 5,
    Food = 6,
    Drink =7,
    HealingPotion = 8,
    ManaPotion = 9,
    IronSword = 10,
    SilverSword = 11,
    OldSword = 12,
    KiteShield = 13,
    RoundShield = 14


}

/// <summary>
/// 아이템 정렬 기준
/// </summary>
public enum ItemSortBy : byte
{
    Code = 0,   // 코드기준
    Name,   // 이름기준
    Price   // 가격기준
}


public enum EquipType : byte
{
    Weapon,
    Shield
}