using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public Animator anim;

    public float maxHealth = 100;
    float currenthealth;

    void Start()
    {
        currenthealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currenthealth -= damage;

        anim.SetTrigger("Hit");

        if (currenthealth < 0 )
        {
            Die();
        }
    }

    void Die()
    {
        anim.SetBool("IsDead", true);
        
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
        this.enabled = false;
    }
}
