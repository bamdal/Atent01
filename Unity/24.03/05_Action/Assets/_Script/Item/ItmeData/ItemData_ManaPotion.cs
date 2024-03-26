using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data - ManaPotion", menuName = "Scriptable Object/Item Data - ManaPotion", order = 5)]
public class ItemData_ManaPotion : ItemData, IUsable
{
    [Header("마나 포션 데이터")]
    /// <summary>
    /// 최대 HP에 비례해서 즉시 회복시켜주는양
    /// </summary>
    public float healRatio = 0.3f;

    public float totalRegen = 1.0f;
    public float duration = 1.0f;

    /// <summary>
    /// 힐링 포션을 사용하기 위한 함수
    /// </summary>
    /// <param name="target">힐링포션의 효과를 받을 대상</param>
    /// <returns>사용 성공 여부</returns>
    public bool Use(GameObject target)
    {
        bool result = false;

        IMana mana = target.GetComponent<IMana>();
        if(mana != null)
        {
            if (mana.MP < mana.MaxMP)
            {
                mana.MP += healRatio * mana.MaxMP;
                mana.ManaRegenerate(totalRegen, duration);
                result = true;
            }
            else
            {
                Debug.Log($"{target.name}의 MP가 가득차 있다.");
            }
        }

        return result;
    }
}
