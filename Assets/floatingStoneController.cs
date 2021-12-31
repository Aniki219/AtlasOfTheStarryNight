using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingStoneController : MonoBehaviour
{
    characterController controller;
    public Vector2 velocity;
    bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector2.zero;
        controller = GetComponent<characterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            if (velocity.x > 0 && controller.collisions.right ||
                velocity.x < 0 && controller.collisions.left ||
                velocity.y > 0 && controller.collisions.above ||
                velocity.y < 0 && controller.collisions.below)
            {
                velocity = Vector2.zero;
                isMoving = false;
            }
        }
    }

    private void FixedUpdate()
    {
        controller.Move(velocity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMoving) return;
        //Maybe add bomberry
        if (collision.CompareTag("AllyHitbox"))
        {
            HitBox hb = collision.GetComponent<AllyHitBoxController>().hitbox;
            if (!hb.broom) return;
            velocity = hb.direction;
        }
    }
}
