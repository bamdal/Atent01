using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealItem : ItemBase
{
    public float heal = 20.0f;

    protected override void OnItemConsum(Player player)
    {
        // hp를 heal만큼 회복
    }
}
