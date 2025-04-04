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
    public AudioSource audioSource;
    public AudioClip attackSound;
    public bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        attackDamage = 10;
        attackTimer = 0;
        attackCD = 2.3f;//与攻击动画时长相关
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            Invoke("DestroyEnemy", 5f);
            return;
        }
        agent.SetDestination(playerController.transform.position);
        if(Vector3.Distance(transform.position , playerController.transform.position) <= 1.0f)
        {
            agent.isStopped = true;
            animator.SetFloat("MoveState", 0);
            if(Time.time - attackTimer >= attackCD)
            {
                attackTimer = Time.time;
                animator.SetTrigger("Attack");
                //playerController.TakeDamage(attackDamage);//调整位置，让血量变化和敌人攻击动作同步
                Invoke("EnemyAttack", 1);

                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                    Invoke("DelayAttackSound", 1);
                }
            }
        }
        else
        {
            agent.isStopped=false;
            animator.SetFloat("MoveState", 1);

            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
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
            isDead = true;
            animator.SetBool("Die", true);
            agent.isStopped = true;
        }
    }

    private void DelayAttackSound()
    {
        audioSource.PlayOneShot(attackSound);
        audioSource.Play();
    }

    private void EnemyAttack()
    {
        playerController.TakeDamage(attackDamage);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
