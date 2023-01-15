using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float minGroundNormalY = .65f, gravityModifier = 1f;

    Rigidbody2D rb;
    protected bool grounded;
    protected Vector2 groundNormal, targetVelocity, velocity;
    ContactFilter2D contactFilter;
    RaycastHit2D[] hitbuffer = new RaycastHit2D[16];
    List<RaycastHit2D>hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistnace = 0.001f, shellRadius = 0.01f;

    public bool CanJump = true;

    [SerializeField] private float rayLength, rayPostionOffset;

    Vector3 RayPostionCenter, RayPostionLeft, RayPostionRight;

    RaycastHit2D[] GroundHitsCenter, GroundHitsRight, GroundHitsLeft;

    RaycastHit2D[][] AllRaycastHits = new RaycastHit2D[3][];


    // Start is called before the first frame update
    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
        CanJump1();
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }
    protected virtual void ComputeVelocity()
    {

    }

    private void FixedUpdate()
    {
        velocity += gravityModifier * Time.deltaTime * Physics2D.gravity;
        velocity.x = targetVelocity.x;

        grounded = false;

        Vector2 deltaPostion = velocity * Time.deltaTime;

        Vector2 moveAlongGrouund = new Vector2(groundNormal.y, -groundNormal.x);

        Vector2 move = moveAlongGrouund * deltaPostion.x;

        Movment(move, false);

        move = Vector2.up * deltaPostion.y;

        Movment(move, true);
    }

    void Movment(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistnace)
        {
            int count = rb.Cast(move, contactFilter, hitbuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitbuffer [i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }

        }

        rb.position = rb.position + move.normalized * distance;
    }
    public void CanJump1()
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
                    if (hit.collider.tag != "Player")
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
