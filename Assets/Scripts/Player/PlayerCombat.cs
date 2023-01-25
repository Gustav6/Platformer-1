using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    private bool combatEnabled;
    [SerializeField]
    private float inputTimer, attack1Radius, attack1Damage;
    [SerializeField]
    private Transform attack1HitBoxPos;
    [SerializeField]
    private LayerMask whatIsDamageable;
    
    private bool gotInput;
    public bool isAttacking;

    private int attackCounter = 0;

    private float lastInputTime = Mathf.NegativeInfinity;

    private bool grounded;
    private bool attack;

    public Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.SetBool("CanAttack", combatEnabled);
    }

    private void Update()
    {
        if (!spriteRenderer.flipX)
        {
            attack1HitBoxPos.localPosition = new Vector2(0.839f, 0.08f);
        }
        else
        {
            attack1HitBoxPos.localPosition = new Vector2(-0.839f, 0.08f);
        }

        grounded = GetComponent<Controller2D>().collisions.below; 

        CheckCombatInput();
        CheckAttacks();
    }


    private void CheckCombatInput()
    {
        if (attack)
        {
            if (combatEnabled)
            {
                gotInput = true;
                lastInputTime = Time.time;
                attack = false;
            }
        }
    }

    private void CheckAttacks()
    {
        if (gotInput)
        {
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                animator.SetInteger("AttackCounter", attackCounter);
                animator.SetBool("Attack1", true);
                animator.SetBool("IsAttacking", isAttacking);
                attackCounter++;
            }
            if (attackCounter >= 3)
            {
                attackCounter = 0;
            }
        }

        if (Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
            attackCounter = 0;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started && grounded)
        {
            attack = true;
        }
    }

    private void CheckAttackHitbox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, whatIsDamageable);

        foreach (Collider2D collider in detectedObjects)
        {
            collider.GetComponent<EnemyHealth>().TakeDamage(attack1Damage);
        }    
    }

    private void FinishAttack1()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("Attack1", false);
    }

    private void AttackCounterReset()
    {
        animator.SetInteger("AttackCounter", 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }
}
