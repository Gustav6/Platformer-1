using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovment : MonoBehaviour
{
    public float horizontalSpeed = 10f;
    public float horizontal;
    public Vector2 movment;
    bool facingRight = true;
    
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
        anim.SetFloat("SpeedX", Mathf.Abs(horizontal));

        Flip();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * horizontalSpeed, rb.velocity.y);
    }

    void Flip()
    {
        if (facingRight && horizontal < 0f || !facingRight && horizontal > 0f)
        {
            Vector3 currentScale = gameObject.transform.localScale;
            currentScale.x *= -1;
            gameObject.transform.localScale = currentScale;

            facingRight = !facingRight;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }
}
