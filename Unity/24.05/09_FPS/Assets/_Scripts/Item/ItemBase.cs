using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : RecycleObject
{
    /// <summary>
    /// 1초에 회전하는 속도
    /// </summary>
    public float spinSpeed = 360.0f;

    /// <summary>
    /// 회전시킬 메시의 트랜스폰
    /// </summary>
    Transform meshTransform;

    /// <summary>
    /// 아이템을 먹었을 때 실행되는 함수
    /// </summary>
    /// <param name="player">아이템을 먹은 플레이어</param>
    protected virtual void OnItemConsum(Player player)
    {

    }

    private void Awake()
    {
        meshTransform = transform.GetChild(0);
    }

    private void Update()
    {
        meshTransform.Rotate(Time.deltaTime * spinSpeed * Vector3.up,Space.World);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(LifeOver(30.0f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                OnItemConsum(player);
                StartCoroutine(LifeOver());
            }
        }


    }
}
