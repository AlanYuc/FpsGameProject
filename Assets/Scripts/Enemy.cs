using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int HP;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
