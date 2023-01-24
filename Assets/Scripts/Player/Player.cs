using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    #region Vertical Movment
    [Header("Veritcal Movment")]

    [Tooltip("How high the player can jump")]
    [SerializeField] float jumpHeight = 4f;
    [Tooltip("How long it takes the player to reach the variabel jumpHeight")]
    [SerializeField] float timeToJumpApex = .4f;
    #endregion

    #region Horizontal Movment
    [Header("Horizontal Movment")]

    [Tooltip("The acceleration then airborne on the x axis")]
    [SerializeField] float accelerationTimeAirborne = .2f;
    [Tooltip("The acceleration when grounded on the x axis")]
    [SerializeField] float accelerationTimeGrounded = .1f;
    [Tooltip("The top speed on the x axis")]
    [SerializeField] float moveSpeed = 6f;
    #endregion

    float gravity;
    float jumpVelocity;
    float velocityXSmothing;

    Vector2 velocity;

    private SpriteRenderer spriteRenderer;

    Controller2D controller;
    void Start()
    {
        controller = GetComponent<Controller2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print ("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
    }
    void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0f;
        }
        Vector2 input = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        bool flipSprite = (spriteRenderer.flipX ? (velocity.x > 0.01f) : (velocity.x < -0.01f));
        if (flipSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move (velocity * Time.deltaTime);
    }
}
