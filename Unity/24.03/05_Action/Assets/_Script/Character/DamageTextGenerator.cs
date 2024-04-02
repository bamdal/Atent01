using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextGenerator : MonoBehaviour
{
    // 이 스크립트를 가진 오브젝트의 부모가 데미지를 받으면 데미지 텍스트 하나씩 생성하는 클래스

    // 부모가 데미지를 받으면 DamageText 프리펩을 하나 생성한다.

    IBattler battler;
    private void Awake()
    {
        battler = GetComponentInParent<IBattler>();
        battler.onHit += DamageTextGenerate;
    }

    private void DamageTextGenerate(int damage)
    {
        Factory.Instance.GetDamageText(damage,transform.position);
    }
}
