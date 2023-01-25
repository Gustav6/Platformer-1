using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    #region Vertical Movment
    [Header("Veritcal Movment")]

    [Tooltip("How high the player can jump")]
    [SerializeField] float jumpHeight = 4f;
    [Tooltip("How long it takes the player to reach the variabel jumpHeight")]
    [SerializeField] float timeToJumpApex = .4f;
    bool jumpPressed;
    #endregion

    #region Horizontal Movment
    [Header("Horizontal Movment")]

    [Tooltip("The acceleration then airborne on the x axis")]
    [SerializeField] float accelerationTimeAirborne = .2f;
    [Tooltip("The acceleration when grounded on the x axis")]
    [SerializeField] float accelerationTimeGrounded = .1f;
    [Tooltip("The top speed on the x axis")]
    [SerializeField] float moveSpeed = 6f;
    float xInput;
    #endregion

    [SerializeField] float gravity, jumpVelocity, velocityXSmothing, velocityYSmothing;

    [SerializeField] Vector2 velocity;

    bool canFlip = true;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    Controller2D controller;
    void Start()
    {
        controller = GetComponent<Controller2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
    }
    void Update()
    {
        bool flipSprite = (spriteRenderer.flipX ? (velocity.x > 0.01f) : (velocity.x < -0.01f));
        if (flipSprite && canFlip)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        animator.SetBool("Grounded", controller.collisions.below);

        if (!controller.collisions.below)
        {
            animator.SetFloat("SpeedY", velocity.y);
        }
        else
        {
            animator.SetFloat("SpeedY", 0);
        }

        if (!jumpPressed)
        {
            
        }

        animator.SetFloat("SpeedX", Mathf.Abs(xInput));

        float targetVelocityX = xInput * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y = Mathf.Clamp(velocity.y, -20, 20);
        velocity.y += gravity * Time.deltaTime;
        controller.Move (velocity * Time.deltaTime);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0f;
        }
    }

    public void XInput(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<Vector2>().x;
    }

    public void YInput(InputAction.CallbackContext context)
    {
        if (context.started && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
            jumpPressed = true;
        }

        if (context.canceled)
        {
            jumpPressed = false;
        }
    }
    public void DisableFlip()
    {
        canFlip = false;
    }

    public void EnableFlip()
    {
        canFlip = true;
    }
}
