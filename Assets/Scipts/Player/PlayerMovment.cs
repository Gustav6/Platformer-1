using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        horizontal = (Input.GetAxis("Horizontal"));

        anim.SetFloat("SpeedX", Mathf.Abs(horizontal));

        flip();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * horizontalSpeed, rb.velocity.y);
    }

    void flip()
    {
        if (facingRight && horizontal < 0f || !facingRight && horizontal > 0f)
        {
            Vector3 currentScale = gameObject.transform.localScale;
            currentScale.x *= -1;
            gameObject.transform.localScale = currentScale;

            facingRight = !facingRight;
        }
    }
}
