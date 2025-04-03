using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int HP;
    public PlayerController playerController;
    public NavMeshAgent agent;
    public int attackDamage;
    public float attackCD;
    public float attackTimer;

    // Start is called before the first frame update
    void Start()
    {
        attackDamage = 10;
        attackTimer = 0;
        attackCD = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playerController.transform.position);
        if(Vector3.Distance(transform.position , playerController.transform.position) <= 1.0f)
        {
            agent.isStopped = true;
            animator.SetFloat("MoveState", 0);
            if(Time.time - attackTimer >= attackCD)
            {
                attackTimer = Time.time;
                animator.SetTrigger("Attack");
                playerController.TakeDamage(attackDamage);
            }
        }
        else
        {
            agent.isStopped=false;
            animator.SetFloat("MoveState", 1);
        }
    }

    /// <summary>
    /// 对敌人造成伤害了
    /// </summary>
    public void TakeDamage(int damageValue)
    {
        animator.SetTrigger("Hurt");
        HP -= damageValue;
        Debug.Log("当前敌人生命值还剩" + HP);
        if (HP <= 0)
        {
            animator.SetBool("Die", true);
        }
    }
}
