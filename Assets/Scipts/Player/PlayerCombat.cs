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

    public Animator anim;
    private bool attack;

    private bool grounded;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("CanAttack", combatEnabled);
    }

    private void Update()
    {
        grounded = GetComponent<PlayerMovment>().isGrounded;

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
            if (attackCounter >= 3)
            {
                attackCounter = 0;
            }

            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                anim.SetInteger("AttackCounter", attackCounter);
                anim.SetBool("Attack1", true);
                anim.SetBool("IsAttacking", isAttacking);
                attackCounter++;
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
            //collider.transform.parent.SendMessage("Damage", attack1Damage);
            collider.GetComponent<EnemyHealth>().TakeDamage(attack1Damage);
        }
    }

    private void FinishAttack1()
    {
        isAttacking = false;
        anim.SetBool("IsAttacking", isAttacking);
        anim.SetBool("Attack1", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }
}
