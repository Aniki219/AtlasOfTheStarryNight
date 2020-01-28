using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    public int moveSpeed = 2;
    BoxCollider2D collider;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = Vector3.zero;
        float hdir = Input.GetAxisRaw("Horizontal");

        anim.SetBool("isRunning", hdir != 0);
        if (hdir != 0)
        {
            translate(ref velocity, transform.right * hdir, moveSpeed);
            transform.localScale = new Vector3(hdir, 1, 1);
        }

        transform.position += velocity;
    }

    Vector2 position2D()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    void translate(ref Vector3 vector, Vector3 direction, int pixels)
    {
        for (int s = pixels; s > 0; s--)
        {
            float dx = s / 32.0f;
            RaycastHit2D boxcast = Physics2D.BoxCast(
                position2D() + collider.offset,
                collider.size * 0.95f,
                0,
                direction,
                dx,
                LayerMask.GetMask("Wall")
            );

            if (!boxcast)
            {
                vector += direction * dx;
                s = 0;
            }
        }
    }
}
