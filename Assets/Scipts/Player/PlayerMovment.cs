using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovment : MonoBehaviour
{
    bool facingRight = true;
    public bool isGrounded = false;

    [SerializeField] 
    Transform GroundCheck;
    [SerializeField] 
    LayerMask groundLayer;
    [SerializeField] 
    private LayerMask jumpableGround;

    private Vector2 newVelocity;
    public Vector2 movment;

    public bool canFlip = true;
    public float xInput;
    public float horizontalSpeed = 10f;
    const float groundCheckRadius = 0.2f;
    public float fallMultiplier = 2.5f;
    public float jumpForce = 10f;


    Rigidbody2D rb;
    public Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }

        anim.SetFloat("SpeedX", Mathf.Abs(xInput));

        Flip();
    }

    void FixedUpdate()
    {
        if (!isGrounded)
        {
            anim.SetFloat("SpeedY", rb.velocity.y);
        }
        else
        {
            anim.SetFloat("SpeedY", 0);
        }

        IsGrounded();
        ApplyMovment();
    }

    void IsGrounded()
    {
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0)
            isGrounded = true;

        anim.SetBool("Jump", !isGrounded);
    }


    public void CheckInput(InputAction.CallbackContext context)
    {
        xInput = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            newVelocity.Set(rb.velocity.x, jumpForce);
            rb.velocity = newVelocity;
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            newVelocity.Set(rb.velocity.x, rb.velocity.y * 0.5f);
            rb.velocity = newVelocity;
        }
    }

    public void ApplyMovment()
    {
        newVelocity.Set(xInput * horizontalSpeed, rb.velocity.y);
        rb.velocity = newVelocity;
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
}
