using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data - Drink", menuName = "Scriptable Object/Item Data - Drink", order = 3)]
public class ItemData_Drink : ItemData, IConsumable
{
    // 획득하면 MP가 있는 대상에게 MP리젠
    // 멤버 변수 : 전체 회복량, 회복시간
    [Header("MP회복 마실것 데이터")]
    public float totalRegen = 1;
    public float duration = 1;

    public void Consume(GameObject target)
    {
        IMana mana = target.GetComponent<IMana>();
        if (mana != null)
        {
            mana.ManaRegenerate(totalRegen, duration);
        }
    }
}
