using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerPlatformerController : PlayerController
{
    public float maxSpeed = 7, JumpSpeed = 7;
    private float xInput;

    private SpriteRenderer spriterenderer;
    private Animator animator;
    void Awake()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = xInput;

        bool flipSprite = (spriterenderer.flipX ? (move.x > 0.01f) : (move.x < -0.01f));
        if (flipSprite)
        {
            spriterenderer.flipX = !spriterenderer.flipX;
        }

        animator.SetBool("Grounded", grounded);
        animator.SetFloat("SpeedX", MathF.Abs (velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;
    }

    public void VerticalMovment(InputAction.CallbackContext context)
    {
        if (context.performed && grounded)
        {
            velocity.y = JumpSpeed;
        }

        if (context.canceled)
        {
            if (velocity.y > 0)
            {
                velocity.y = velocity.y * .5f;
            }
        }
    }
    public void CheckInput(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<Vector2>().x;
    }
}
