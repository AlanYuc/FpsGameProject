using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int HP;
    public Transform playerPos;
    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(playerPos.position);
    }

    /// <summary>
    /// �Ե�������˺���
    /// </summary>
    public void TakeDamage(int damageValue)
    {
        animator.SetTrigger("Hurt");
        HP -= damageValue;
        Debug.Log("��ǰ��������ֵ��ʣ" + HP);
        if (HP <= 0)
        {
            animator.SetBool("Die", true);
        }
    }
}
