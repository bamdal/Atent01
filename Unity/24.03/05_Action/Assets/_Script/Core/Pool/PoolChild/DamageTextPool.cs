using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageTextPool : ObjectPool<DamageText>
{
    Vector3 p;

    /// <summary>
    /// 데미지 텍스트 하나를 소환하는 함수
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <param name="position">생성될 장소</param>
    /// <returns>소환된 오브젝트</returns>
    public GameObject GetObject(int damage,Vector3? position)
    {
        DamageText damageText = GetObject(position);
        damageText.SetDamage(damage);

        return damageText.gameObject;
    }


}
