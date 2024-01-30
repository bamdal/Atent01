using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBase : MonoBehaviour
{
    /// <summary>
    /// 터렛이 발사할 총알
    /// </summary>
    public PoolObjectType bulletType = PoolObjectType.Bullet;

    /// <summary>
    /// 총알 발사 위치 설정용 트랜스폼
    /// </summary>
    protected Transform fireTransform;

    /// <summary>
    /// 총알 발사 간격
    /// </summary>
    public float fireInterval = 1.0f;

    protected virtual void Awake()
    {
        Transform child = transform.GetChild(2);
        fireTransform = child.GetChild(1);
    }




}
