using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float health;
    
    public float maxHealth = 100;
    public bool isDead = false;

    public Animator anim;
    public Image healthBar;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        if (Input.GetKeyDown(KeyCode.Insert))
        {
            Heal(5);
        }

        if (isDead)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.fillAmount = health / 100f;
        anim.SetTrigger("PlayerTookDamage");
        if (health <= 0)
        {
            GetComponent<PlayerMovment>().enabled = false;
            GetComponent<PlayerJump>().enabled = false;
            GetComponent<PlayerAttack>().enabled = false;
            anim.SetBool("IsDead", true);
            anim.SetBool("Jump", false);
            isDead = true;
        }
    }

    public void Heal(float healingAmount)
    {
        health += healingAmount;
        health = Mathf.Clamp(health, 0, maxHealth);

        healthBar.fillAmount = health / 100f;
    }
}
