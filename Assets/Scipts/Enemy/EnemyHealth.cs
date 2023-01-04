using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public Animator anim;

    public int maxHealth = 100;
    int currenthealth;

    // Start is called before the first frame update
    void Start()
    {
        currenthealth = maxHealth;
    }

    public void TakeDamage(int damage)
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
        this.enabled= false;
    }

}
