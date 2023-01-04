using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerJump : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] Transform GroundCheck;
    [SerializeField] LayerMask groundLayer;

    const float groundCheckRadius = 0.2f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float jumpForce = 10f;
    [SerializeField] private bool isGrounded = false;

    [SerializeField] public LayerMask jumpableGround;
    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.up * jumpForce;
            anim.SetBool("Jump", true);
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier -1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
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

        Grounded();
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
