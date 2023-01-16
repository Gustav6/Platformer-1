using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovment : MonoBehaviour
{

    [SerializeField] private Transform wallCheck;
    [SerializeField] LayerMask groundLayer, jumpableGround, wallLayer;

    // Checks if you can flip the player
    bool facingRight = true, canFlip = true;

    // Horizontal movemnt speed and the input
    public float xInput, horizontalSpeed = 10f;

    // Jump, checks if you are on the gorund and custom gravity
    private bool isJumpingFromWall = false;
    const float groundCheckRadius = 0.2f;
    public float fallMultiplier = 2.5f, jumpForce = 10f;

    // Allows you too jump after you have left a ledge
    public float hangTime = .1f;
    private float hangCounter;

    // Dash
    public float dashingPower = 10f, dashingTime = 0.2f, dashingCooldown = 1f;
    bool isDashing, canDash = true;
    [SerializeField] private TrailRenderer tr;
    // Wall slide
    private bool isWallSliding;
    public float wallSlidingSpeed = 2f;

    // Wall Jump
    [SerializeField] private bool isWallJumping;
    [SerializeField] private float wallJumpingTime = 0.2f, wallJumpingDuration = 0.4f;
    private float wallJumpingDirection, wallJumpingCounter;
    public Vector2 wallJumpingPower = new(8f, 16f);

    // Double jump
    [SerializeField] private bool doubleJump;

    // Checks if player is attack and chages horizontal speed based on that
    private bool isAttacking;
    public float attackHorizontalSpeed = 0f;

    // Raycasts for grounded
    [SerializeField] private float rayLength, rayPostionOffset;

    Vector3 RayPostionCenter, RayPostionLeft, RayPostionRight;

    RaycastHit2D[] GroundHitsCenter, GroundHitsRight, GroundHitsLeft;

    RaycastHit2D[][] AllRaycastHits = new RaycastHit2D[3][];

    public bool CanJump = true;

    // Calls for components outside script
    public ParticleSystem footsteps;
    private ParticleSystem.EmissionModule footEmission;
    Rigidbody2D rb;
    public Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        footEmission = footsteps.emission;
    }

    void Update()
    {
    

        Grounded();

        isAttacking = GetComponent<PlayerCombat>().isAttacking;

        if (isDashing)
        {
            return;
        }

        if (CanJump || doubleJump)
        {
            hangCounter = hangTime;
        }
        else
        {
            hangCounter -= Time.deltaTime;
        }

        if (xInput != 0 && CanJump)
        {
            footEmission.rateOverTime = 35f;
        }
        else
        {
            footEmission.rateOverTime = 0f;
        }

        if (rb.velocity.y < 0f)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }

        anim.SetFloat("SpeedX", Mathf.Abs(xInput));

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

        if (!CanJump)
        {
            anim.SetFloat("SpeedY", rb.velocity.y);
        }
        else
        {
            anim.SetFloat("SpeedY", 0);
        }

        WallJump();
        WallSlide();
        if (!isWallJumping)
        {
            ApplyMovment();
        }
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !CanJump && xInput != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2 (rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
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
        if (DialogueManager.isActive == true)
        {
            return;
        }

        if (context.performed && canDash && !isWallSliding)
        {
            StartCoroutine(Dash());
        }
    }

    public void CheckInput(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<Vector2>().x;
    }

    public void VerticalMovment(InputAction.CallbackContext context)
    {
        if (DialogueManager.isActive == true)
        {
            return;
        }

        if (context.performed && hangCounter > 0f && !isWallSliding)
        {
            rb.velocity = new Vector2 (rb.velocity.x, jumpForce);

            doubleJump = !doubleJump;
        }

        if (context.started && doubleJump)
        {
            rb.velocity = new Vector2 (rb.velocity.x, jumpForce);
        }

        if (context.canceled && rb.velocity.y > 0f && !isWallSliding)
        {
            rb.velocity = new Vector2 (rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (isWallSliding)
        {
            isJumpingFromWall = true;
        }

        if (CanJump && !context.performed)
        {
            doubleJump = false;
        }
    }

    public void Grounded()
    {
        RayPostionCenter = transform.position + new Vector3(0, -0.75f, 0);
        RayPostionLeft = transform.position + new Vector3(-rayPostionOffset, -0.75f, 0);
        RayPostionRight = transform.position + new Vector3(rayPostionOffset, -0.75f, 0);

        GroundHitsCenter = Physics2D.RaycastAll(RayPostionCenter, Vector2.down, rayLength);
        GroundHitsLeft = Physics2D.RaycastAll(RayPostionLeft, Vector2.down, rayLength);
        GroundHitsRight = Physics2D.RaycastAll(RayPostionRight, Vector2.down, rayLength);

        AllRaycastHits[0] = GroundHitsCenter;
        AllRaycastHits[1] = GroundHitsLeft;
        AllRaycastHits[2] = GroundHitsRight;

        CanJump = GroundChecks(AllRaycastHits);

        Debug.DrawRay(RayPostionCenter, Vector2.down * rayLength, Color.red);
        Debug.DrawRay(RayPostionLeft, Vector2.down * rayLength, Color.red);
        Debug.DrawRay(RayPostionRight, Vector2.down * rayLength, Color.red);
    }

    private bool GroundChecks(RaycastHit2D[][] GroundHits)
    {
        foreach (RaycastHit2D[] Hitlist in GroundHits)
        {
            foreach (RaycastHit2D hit in Hitlist)
            {
                if (hit.collider != null)
                {
                    if (!hit.collider.CompareTag("Player"))
                    {
                        anim.SetBool("Grounded", true);
                        return true;
                    }
                }
            }
        }
        anim.SetBool("Grounded", false);
        return false;
    }

    public void ApplyMovment()
    {
        if (DialogueManager.isActive == true)
        {
            return;
        }
       

        if (!isAttacking)
        {
            rb.velocity = new Vector2 (xInput * horizontalSpeed, rb.velocity.y);
        }
        if (isAttacking)
        {
            rb.velocity = new Vector2 (xInput * horizontalSpeed * attackHorizontalSpeed, rb.velocity.y);
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
}
