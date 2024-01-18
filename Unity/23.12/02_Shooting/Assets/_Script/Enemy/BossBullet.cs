using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossBullet : RecycleObject
{

    public float bulletSpeed = 7.0f;


    public float LifeTime = 10.0f;


    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(LifeOver(LifeTime));
    }

    void Update()
    {

        transform.Translate(Time.deltaTime * bulletSpeed * Vector2.left); 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Factory.Instance.GetHitEffect(collision.contacts[0].point);
            gameObject.SetActive(false);

        }


    }
}
