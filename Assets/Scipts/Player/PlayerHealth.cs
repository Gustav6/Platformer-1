using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Start is called before the first frame update

    public Animator anim;
    public int maxHealth = 100;
    int health;
    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        anim.SetTrigger("PlayerTookDamage");
        if (health <= 0)
        {
            anim.SetBool("IsDead", true);
        }
    }
}
