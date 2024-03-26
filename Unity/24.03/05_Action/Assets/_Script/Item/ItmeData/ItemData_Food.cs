using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data - Food", menuName = "Scriptable Object/Item Data - Food", order = 2)]
public class ItemData_Food : ItemData, IConsumable
{
    // 획득하면 HP가 있는 대상에게 HP증가 - 틱으로 리젠
    // 맴버 변수로 틱당 회복량 틱 인터벌 , 틱횟수
    [Header("HP회복 음식 데이터")]
    public float tickRegen = 1.0f;
    public float tickInterval = 1.0f;
    public uint totalTickCount = 1;
    public void Consume(GameObject target)
    {
        IHealth health = target.GetComponent<IHealth>();
        if (health != null)
        {
            health.HealthRegenerateByTick(tickRegen, tickInterval, totalTickCount);
        }
    }
}
