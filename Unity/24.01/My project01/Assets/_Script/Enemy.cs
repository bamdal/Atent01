using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Astar
{
    Animator animator;
    
    public float enemySpeed = 7.0f;
    Astar astar;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        astar = GetComponent<Astar>();
        astar.FindPlayer += IsAttack;
        
        astar.animdirection += AnimDirection;
    }

    private void AnimDirection(float direction)
    {
        Debug.Log("방향");
        if (direction > 0)
        {
            // Flip the sprite to face right
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        // Check if the direction is towards the left (negative x)
        else if (direction < 0)
        {
            // Flip the sprite to face left
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        animator.SetBool("IsMoving", direction != 0);
    }

    private void IsAttack()
    {
        Debug.Log("Enemy 공격");
    }
} 
