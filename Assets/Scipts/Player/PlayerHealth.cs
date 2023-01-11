using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth;
    public float maxHealth = 100;
    public bool isDead = false;

    public Animator anim;
    public HealthBar healthBar;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.MaxHealth(maxHealth);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
        {
            Heal(5);
        }

        if (isDead)
        {
            rb.velocity = new Vector2(0, -1);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth); 
        if (currentHealth >= 0.1f)
        {
        anim.SetTrigger("PlayerTookDamage");
        }

        if (currentHealth <= 0 && !isDead)
        {
            GetComponent<PlayerMovment>().enabled = false;
            GetComponent<PlayerCombat>().enabled = false;
            anim.SetBool("IsDead", true);
            anim.SetBool("Jump", false);
            isDead = true;
        }
    }

    public void Heal(float healingAmount)
    {
        currentHealth += healingAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
