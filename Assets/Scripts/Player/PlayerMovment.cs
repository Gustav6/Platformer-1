using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovment : MonoBehaviour
{
    [SerializeField] 
    Transform GroundCheck;
    [SerializeField] 
    LayerMask groundLayer;
    [SerializeField] 
    private LayerMask jumpableGround;
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private LayerMask wallLayer;

    private Vector2 newVelocity;

    // Checks if you can flip the player
    bool facingRight = true;
    public bool canFlip = true;

    // Horizontal movemnt speed and the input
    public float xInput;
    public float horizontalSpeed = 10f;

    // Jump, checks if you are on the gorund and custom gravity
    private bool isJumpingFromWall = false;
    public bool isGrounded = false;
    const float groundCheckRadius = 0.2f;
    public float fallMultiplier = 2.5f;
    public float jumpForce = 10f;

    // Allows you too jump after you have left a ledge
    public float hangTime = .1f;
    private float hangCounter;

    Rigidbody2D rb;
    public Animator anim;

    // Dash
    public float dashingPower = 10f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    bool isDashing;
    bool canDash = true;
    [SerializeField]
    private TrailRenderer tr;

    // Particales when walking
    public ParticleSystem footsteps;
    private ParticleSystem.EmissionModule footEmission;

    // Wall slide
    private bool isWallSliding;
    public float wallSlidingSpeed = 2f;

    // Wall Jump
    [SerializeField]
    private bool isWallJumping;
    private float wallJumpingDirection;
    [SerializeField]
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    [SerializeField]
    private float wallJumpingDuration = 0.4f;
    public Vector2 wallJumpingPower = new(8f, 16f);

    // Double jump
    [SerializeField]
    private bool doubleJump;

    // Checks if player is attack and chages horizontal speed based on that
    private bool isAttacking;
    public float attackHorizontalSpeed = 0f;

    // How to handle slopes

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        footEmission = footsteps.emission;
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }

        isAttacking = GetComponent<PlayerCombat>().isAttacking;

        if (isGrounded || doubleJump)
        {
            hangCounter = hangTime;
        }
        else
        {
            hangCounter -= Time.deltaTime;
        }

        if (xInput != 0 && isGrounded)
        {
            footEmission.rateOverTime = 35f;
        }
        else
        {
            footEmission.rateOverTime = 0f;
        }

        if (rb.velocity.y < 0 && !isWallSliding)
        {
            rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }

        anim.SetFloat("SpeedX", Mathf.Abs(xInput));

        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        if (!isGrounded)
        {
            anim.SetFloat("SpeedY", rb.velocity.y);
        }
        else
        {
            anim.SetFloat("SpeedY", 0);
        }

        IsGrounded();
        if (!isWallJumping)
        {
            ApplyMovment();
        }
    }

    void IsGrounded()
    {
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0)
            isGrounded = true;

        anim.SetBool("Jump", !isGrounded);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !isGrounded && xInput != 0f)
        {
            isWallSliding = true;
            newVelocity.Set(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            rb.velocity = newVelocity;
        }
        else
        {
            isWallSliding = false;
        }
    }


    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
            anim.SetBool("WallSlide", true);

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            anim.SetBool("WallSlide", false);
            wallJumpingCounter -= Time.deltaTime;
        }

        if (isJumpingFromWall && wallJumpingCounter > 0f)
        {
            isJumpingFromWall = false;
            isWallJumping = true;
            rb.velocity = new(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                facingRight = !facingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }
    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    public void CheckDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && !isWallSliding)
        {
            StartCoroutine(Dash());
        }
    }

    public void CheckInput(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && hangCounter > 0f && !isWallSliding)
        {
            newVelocity.Set(rb.velocity.x, jumpForce);
            rb.velocity = newVelocity;

            doubleJump = !doubleJump;
        }

        if (context.started && doubleJump)
        {
            newVelocity.Set(rb.velocity.x, jumpForce);
            rb.velocity = newVelocity;
        }

        if (context.canceled && rb.velocity.y > 0f && !isWallSliding)
        {
            newVelocity.Set(rb.velocity.x, rb.velocity.y * 0.5f);
            rb.velocity = newVelocity;
        }

        if (isWallSliding)
        {
            isJumpingFromWall = true;
        }

        if (isGrounded && !context.performed)
        {
            doubleJump = false;
        }
    }

    public void ApplyMovment()
    {
        if (!isAttacking)
        {
            newVelocity.Set(xInput * horizontalSpeed, rb.velocity.y);
            rb.velocity = newVelocity;
        }
        if (isAttacking)
        {
            newVelocity.Set(xInput * horizontalSpeed * attackHorizontalSpeed, rb.velocity.y);
            rb.velocity = newVelocity;
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

    void Flip()
    {
        if (facingRight && xInput < 0f  && canFlip || !facingRight && xInput > 0f && canFlip)
        {
            Vector3 currentScale = gameObject.transform.localScale;
            currentScale.x *= -1;
            gameObject.transform.localScale = currentScale;

            facingRight = !facingRight;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(wallCheck.position, 0.2f);
    }

    private void SlowMovmentOnHit()
    {
        horizontalSpeed *= 0.2f;
    }
    private void ReturnMovmentAfterHit()
    {
        horizontalSpeed = 8.5f;
    }
}
