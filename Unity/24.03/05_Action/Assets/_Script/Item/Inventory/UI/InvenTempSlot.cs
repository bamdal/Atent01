using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvenTempSlot : InvenSlot
{
    const uint NotSet = uint.MaxValue;

    uint fromIndex = NotSet;

    public uint FromIndex => fromIndex;


    public InvenTempSlot(uint index) : base(index)
    {
        fromIndex = NotSet;
    }

    //public override void AssignSlotItem(ItemData data, uint count = 1, bool isEquipped = false, uint? from = null)
    //{
    //    base.AssignSlotItem(data, count, isEquipped, fromIndex);
    //    fromIndex = from ?? NotSet; // ?? 이 null이면 가지고 있는값, null이면 뒤에있는 값
    //}

    public override void ClearSlotItem()
    {
        base.ClearSlotItem();
        fromIndex = NotSet;
    }

    public void SetFromIndex(uint index)
    {
        fromIndex = index;
    }
}
