using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    Enemy enemy;
    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        //Hit = enemy.HitEnemy();
    }
    public Action Hit;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttack"))
        {
            enemy.HitEnemy();

        }
    }
}
