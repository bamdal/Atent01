using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Astar
{
    Animator animator;
    public float enemySpeed = 7.0f;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

} 
