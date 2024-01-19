using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Astar
{
    Animator animator;

    public float enemyDamage = 1.0f;
    public float enemySpeed = 7.0f;
    public float maxHp = 3.0f;
    float hp = 3.0f;
    Astar astar;
    List<Node> FinalList;
    readonly int Enemy_Move = Animator.StringToHash("IsMoving");
    readonly int Enemy_Attack = Animator.StringToHash("IsAttack");
    readonly int Enemy_Hit = Animator.StringToHash("IsHit");




    public float Hp
    {
        get => hp;
        private set
        {
            if (hp != value)
            {
                Debug.Log("맞음");
            }
            hp = value;
            hp = Math.Clamp(value, 0.0f, maxHp);
            if (hp == 0.0f)
            {
                OnDie();
            }
        }
    }



    /*    private float GetAnimLength(string animName)
        {
            float time = 0;
            RuntimeAnimatorController ac = animator.runtimeAnimatorController;

            for (int i = 0; i < ac.animationClips.Length; i++)
            {
                if (ac.animationClips[i].name == animName)
                {
                    time = ac.animationClips[i].length;
                }
            }

            return time;
        }*/

    private void Awake()
    {
        animator = GetComponent<Animator>();
        astar = GetComponent<Astar>();
        player = FindAnyObjectByType<Player>();
        hp = maxHp;
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
        if (collision.CompareTag("Player"))
        {
            Debug.Log("멈춤");
            animator.SetBool(Enemy_Move, false);
            StartCoroutine(EnemyAttack());
            astar.astarmove = true;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !astar.astarmove)
        {
            Debug.Log("멈춤");
            animator.SetBool(Enemy_Move, false);
            StartCoroutine(EnemyAttack());
            astar.astarmove = true;
        }
    }

    public void HitEnemy()
    {
        StopAllCoroutines();
        Hp--;
        animator.SetBool(Enemy_Hit, true);
        astar.astarmove = true;
        StartCoroutine(Delay());
    }

    IEnumerator Delay() 
    {
        yield return new WaitForSeconds(0.333f);
        animator.SetBool(Enemy_Hit, false);
        animator.SetBool(Enemy_Move, true);
        animator.SetBool(Enemy_Attack, false);
        PathFinding();
        astar.astarmove = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetBool(Enemy_Move, true);
            animator.SetBool(Enemy_Attack, false);
            astar.astarmove = false;
        }
        
    }



    IEnumerator EnemyAttack()
    {
        animator.SetBool(Enemy_Attack, true);

        yield return new WaitForSeconds(0.667f);

        animator.SetBool(Enemy_Attack, false);
        astar.astarmove = false;
    }
    private void IsAttack()
    {
        Debug.Log("Enemy 공격");
    }

    private void OnDie()
    {
        Debug.Log("사망");
        gameObject.SetActive(false);
    }

    public void EnemyAttacking()
    {
        Debug.Log(Vector3.SqrMagnitude(player.transform.position- transform.position));
        if (Vector3.SqrMagnitude(player.transform.position - transform.position) < 5.0f)
        {
            player.PlayerHit(enemyDamage);
        }
    }

}
