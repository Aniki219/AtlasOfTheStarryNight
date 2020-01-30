using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] private int moveSpeed = 4;
    [SerializeField] float fallRate = 23f;
    float fallSpeed = 0;
    Animator anim;
    new BoxCollider2D collider;

    Vector3 velocity;
    bool jump = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            jump = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float hdir = Input.GetAxisRaw("Horizontal");

        anim.SetBool("isRunning", hdir != 0);
        if (hdir != 0)
        {
            translate(transform.right * hdir, moveSpeed * Time.fixedDeltaTime);
            transform.localScale = new Vector3(hdir, 1, 1);
        }
        translate(transform.up * -1 * Mathf.Sign(fallSpeed), Mathf.Abs(fallSpeed * Time.fixedDeltaTime));
        if (!isGrounded())
        {
            fallSpeed += fallRate * Time.fixedDeltaTime;
        } else
        {
            if (fallSpeed > 0)
            {
                fallSpeed = 0;
            }

            if (jump)
            {
                fallSpeed = -10f;
                jump = false;
            }
        }
    }

    void translate(Vector3 direction, float dx)
    {
        RaycastHit2D boxcast = Physics2D.BoxCast(
            (Vector2)(transform.position + direction * Time.fixedDeltaTime) + collider.offset,
            collider.size * 0.95f,
            0,
            direction,
            dx,
            LayerMask.GetMask("Wall")
        );

        if (!boxcast)
        {
            transform.position += direction * dx;
        } else
        {
            transform.position += boxcast.distance * direction;
        }
    }

    bool isGrounded()
    {
        RaycastHit2D boxcast = Physics2D.BoxCast(
            (Vector2)transform.position + collider.offset,
            collider.size * 0.95f,
            0,
            transform.up * -1,
            Time.fixedDeltaTime,
            LayerMask.GetMask("Wall")
        );

        return (bool)boxcast;
    }
}
