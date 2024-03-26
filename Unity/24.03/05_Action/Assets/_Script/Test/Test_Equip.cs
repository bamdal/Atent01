using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Equip : TestBase
{
#if UNITY_EDITOR

    private void Start()
    {
        Factory.Instance.MakeItem(ItemCode.IronSword);
        Factory.Instance.MakeItem(ItemCode.SilverSword);
        Factory.Instance.MakeItem(ItemCode.OldSword);
    }
#endif
}
