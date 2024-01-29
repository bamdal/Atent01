using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : RecycleObject
{
    public float initalSpeed = 20.0f;
    public float lifeTime = 10.0f;

    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        rigid.velocity = initalSpeed * transform.forward;

        StartCoroutine(a());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    IEnumerator a()
    {
        yield return new WaitForSeconds(lifeTime);
        this.gameObject.SetActive(false);

    }
}
