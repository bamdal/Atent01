using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPool : ObjectPool<Boss>
{
    protected override void OnGetObject(Boss component)
    {
        Vector3 pos = component.transform.position;
        pos.y = 0;
        component.transform.position = pos; // 보스는 무조건 가운데서 등장

        component.OnSpawn();// 스폰 마무리
    }
}
