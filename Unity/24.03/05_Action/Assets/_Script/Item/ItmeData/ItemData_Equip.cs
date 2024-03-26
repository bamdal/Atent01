using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData_Equip : ItemData, IEquipable
{
    [Header("장비 아이템 데이터")]
    /// <summary>
    /// 아이템을 장비 했을 때 플레이어 모델에 추가될 프리팹
    /// </summary>
    public GameObject equipPrefab;

    /// <summary>
    /// 아이템 장비될 위치를 알려주는 프로퍼티
    /// </summary>
    public virtual EquipType EquipType => EquipType.Weapon;

    /// <summary>
    /// 아이템을 장착하는 함수
    /// </summary>
    /// <param name="target">장착받을 대상</param>
    /// <param name="slot">장착할 아이템이 들어있는 슬롯</param>
    public void Equip(GameObject target, InvenSlot slot)
    {
        IEquipTarget equipTarget = target.GetComponent<IEquipTarget>();
        if (equipTarget != null)
        {
            equipTarget.EquipItem(EquipType,slot);
        }
    }

    /// <summary>
    /// 아이템을 장착 해제하는 함수
    /// </summary>
    /// <param name="target">장착 해제할 대상</param>
    /// <param name="slot">장착 해제할 아이템이 들어있는 슬롯</param>
    public void UnEquip(GameObject target, InvenSlot slot)
    {
        IEquipTarget equipTarget = target.GetComponent<IEquipTarget>();
        if (equipTarget != null)
        {
            equipTarget.UnEquipItem(EquipType);
        }
    }


    /// <summary>
    /// 상황에 맞게 장착 또는 장착 해제를 하는 함수
    /// </summary>
    /// <param name="target">대상</param>
    /// <param name="slot">아이템이 들어있는 슬롯</param>
    public void ToggleEquip(GameObject target, InvenSlot slot)
    {
        IEquipTarget equipTarget = target.GetComponent<IEquipTarget>();
        if (equipTarget != null)
        {
            InvenSlot oldSlot = equipTarget[EquipType];
            if(oldSlot == null) 
            {
                // 아무것도 장비 되어있지 않다
                Equip(target, slot);    // 입력 받은 것 장비
            }
            else
            {
                // 무언가 장비되어있다.
                UnEquip(target, oldSlot);   // 이전 것 장비 해제
                if (oldSlot != slot)
                {
                    Equip(target, slot);    // 다른 슬롯에 있는 장비를 클릭한 것이였으면 그 슬롯에 있는 것 장비
                }
            }
        }
    }

}
