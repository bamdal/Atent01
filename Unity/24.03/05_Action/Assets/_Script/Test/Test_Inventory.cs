using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Inventory : MonoBehaviour
{
    ItemData itemData;
    void Start()
    {
        itemData = GameManager.Instance.ItemDataManager.itemDatas[(int)ItemCode.Ruby];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
