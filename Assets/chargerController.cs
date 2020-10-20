using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chargerController : enemyAI
{
    public bool readyToPounce = false;
    public float homeX;
    public float targetX;
    public float leashRange = 3.0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        state.addStates("Snarl", "Charge", "StopSlide", "Plop", "Bonk", "Pop", "Roll");
        intangibleStates.AddRange(state.findStates("Plop", "Bonk"));

        actionCoolDown = 3.0f;
        act = (readyToPounce) ? 0 : actionCoolDown;
        targetX = homeX = transform.position.x;
    }

    void Update()
    {
        currentState = state.getState();
        switch (state.getState())
        {
            case "Movement":
                handleMovement();
                applyGravity();
                break;
            case "Charge":
                handleCharge();
                applyGravity();
                break;
            default:
                applyGravity();
                break;
        }
    }

    // Update is called once per frame
    void handleMovement()
    {
        anim.SetBool("isFalling", !isGrounded && (velocity.y < -1.0f));
        anim.SetBool("isWalking", isGrounded && (Mathf.Abs(velocity.x) > 0));
        
        if (lineOfSight() && isGrounded)
        {
            triggerState("Snarl");
        }
        
        if (act <= 0)
        {
            targetX = homeX + Random.Range(-leashRange, leashRange);
            act = actionCoolDown;
        } else
        {
            act -= Time.deltaTime;
        }

        float dir = Mathf.Sign(targetX - transform.position.x);
        float dist = Mathf.Abs(targetX - transform.position.x);
        if (dist > moveSpeed * 2.0f * Time.deltaTime)
        {
            velocity.x = moveSpeed * dir;
        } else
        {
            velocity.x = 0;
        }

        if (velocity.y < maxFallVel) { velocity.y = maxFallVel; }
    }

    void handleCharge()
    {

    }

    void applyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    void jump()
    {
        deformer.startDeform(new Vector3(0.9f, 1.1f, 1.0f), 0.1f, 0.1f);
        velocity.x = 2.5f * facing;
        velocity.y = 3f;
    }

    void attack()
    {
        velocity.x = 4f * facing;
        velocity.y = 3f;
        anim.SetTrigger("Attack");
    }

    void setFacing()
    {
        if (!isGrounded) return;
        transform.localScale = new Vector3(facing * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        controller.Move(velocity * Time.deltaTime);
        if (controller.collisions.above || controller.collisions.below)
        {
            if (anim.GetBool("isFalling"))
            {
                deformer.startDeform(new Vector3(1.2f, 0.75f, 1.0f), 0.1f, 0.1f);
                if (velocity.y <= -4f)
                {
                    triggerState("Plop");
                    Invoke("returnToIdle", 1.5f);
                }
                else
                {
                    if (velocity.y < -1f)
                    {
                        deformer.startDeform(new Vector3(1.2f, 0.75f, 1.0f), 0.1f, 0.1f);
                    }
                    returnToMovement();
                }
            }
            velocity.y = 0;
        }
        if ((controller.collisions.right && velocity.x > 0) || (controller.collisions.left && velocity.x < 0))
        {
            velocity.x = 0;
            if (velocity.y > 0)
            {
                velocity.y = 0;
            }
        }
    }
}