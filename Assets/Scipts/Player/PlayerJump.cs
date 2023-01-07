using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D cc;
    private Animator anim;
    [SerializeField] Transform GroundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] private float slopeCheckDistance;
    private float slopeDownAngle;
    private float slopeDownAngleOld;

    private Vector2 slopeNormalPerp;
    private Vector2 colliderSize;
    const float groundCheckRadius = 0.2f;
    public float fallMultiplier = 2.5f;
    public float jumpForce = 10f;

    private bool isOnSlope;
    public bool isGrounded = false;
    public LayerMask jumpableGround;
    // Start is called before the first frame update
    void Awake()
    {
        cc = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        colliderSize = cc.size;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    void Update()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }
    }

    private void FixedUpdate()
    {
        if (!isGrounded)
        {
            anim.SetFloat("SpeedY", rb.velocity.y);
        }
        else
        {
            anim.SetFloat("SpeedY", 0);
        }

        SlopeCheck();
        Grounded();
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);

        SlopeCheckVertical(checkPos);
    } 

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {

    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal);

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
    }

    void Grounded()
    {
        isGrounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundCheck.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0) 
            isGrounded = true;

        anim.SetBool("Jump", !isGrounded);
    }
}
