using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;

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
    public void TakeDamage()
    {
        animator.SetTrigger("Hurt");
    }
}
