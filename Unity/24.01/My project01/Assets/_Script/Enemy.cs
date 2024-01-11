using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Astar
{
    Animator animator;

    public float enemySpeed = 7.0f;
    Astar astar;
    List<Node> FinalList;
    readonly int Enemy_Move = Animator.StringToHash("IsMoving");
    private void Awake()
    {
        animator = GetComponent<Animator>();
        astar = GetComponent<Astar>();
        PathFinding();
    }

    protected override void PathFinding()
    {

        base.PathFinding();
        FinalList = astar.FinalNodeList;
        StartCoroutine(OnMove());
    }

    IEnumerator OnMove()
    {
        astarmove = false;

        foreach (var node in FinalList)
        {
            Vector3 targetPosition = new Vector3(node.x, node.y, transform.position.z);

            while (transform.position != targetPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                AnimDirection(targetPosition.x - transform.position.x);

                yield return null;
            }

          
        }
        animator.SetBool(Enemy_Move, false);
        yield return StartCoroutine(WaitForPathFinding());
    }
    IEnumerator WaitForPathFinding()
    {
        // OnMove 코루틴이 끝날 때까지 기다리기
        yield return new WaitUntil(() => astarmove == false);

        // 모든 노드 이동이 끝난 후에 실행할 로직 추가 가능
        PathFinding();
    }
    private void AnimDirection(float direction)
    {


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
        animator.SetBool(Enemy_Move, true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("멈춤");
        animator.SetBool(Enemy_Move, false);
        astar.astarmove = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        animator.SetBool(Enemy_Move, true);
        astar.astarmove = false;
    }

    private void IsAttack()
    {
        Debug.Log("Enemy 공격");
    }
}
