using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    #region Players Vertical Input/Movment
    [Header("Veritcal Movment")]

    [Tooltip("Maximum height the player can jump")]
    [SerializeField] float maxJumpHeight = 4f;
    [Tooltip("Minimum height the player can jump")]
    [SerializeField] float minJumpHeight = 2f;
    [Tooltip("How long it takes the player to reach the variabel jumpHeight")]
    [SerializeField] float timeToJumpApex = .4f;
    [Tooltip("The maximum amount of jump speed for the player")]
    [SerializeField] float maxJumpVelocity;
    [Tooltip("The minimum amount of jump speed for the player")]
    [SerializeField] float minJumpVelocity;
    #endregion

    #region Players Horizontal Input/Movment
    [Header("Horizontal Movment")]

    [Tooltip("The acceleration then airborne on the x axis")]
    [SerializeField] float accelerationTimeAirborne = .2f;
    [Tooltip("The acceleration when grounded on the x axis")]
    [SerializeField] float accelerationTimeGrounded = .1f;
    [Tooltip("The top speed on the x axis")]
    [SerializeField] float moveSpeed = 6f;
    [Tooltip("Acceleration for the player on the x axis")]
    [SerializeField] float velocityXSmothing;
    // players input on the x axis
    float xInput;
    #endregion

    #region Player Logic
    [Header("Player Logic")]

    [Tooltip("How high the gravity it will be on the player")]
    [SerializeField] float gravity;
    [Tooltip("The players velocity on the x and y axis")]
    [SerializeField] Vector2 velocity;
    #endregion

    // Variables for the players sprite
    bool canFlip = true;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    Controller2D controller;
    void Start()
    {
        controller = GetComponent<Controller2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity)) * minJumpHeight;
        print("Gravity: " + gravity + " Jump Velocity: " + maxJumpVelocity);
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
            velocity.y = maxJumpVelocity;
        }

        if (context.canceled && velocity.y > 0)
        {
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }
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
