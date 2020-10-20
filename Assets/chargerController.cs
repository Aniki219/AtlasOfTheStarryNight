using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chargerController : enemyAI
{
    public bool readyToPounce = false;
    public float homeX;
    public float targetX;
    public float leashRange = 3.0f;
    public float chargeVelocity = 4.0f;

    float chargeStart = Mathf.Infinity;
    public float chargeDuration = 3.0f;

    float lastCharge = -10;
    public Transform starOrigin;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        state.addStates("Snarl", "Charge", "Slide", "Plop", "Bonk", "Pop", "Roll");
        intangibleStates.AddRange(state.findStates("Plop", "Bonk"));

        actionCoolDown = 3.0f;
        act = (readyToPounce) ? 0 : actionCoolDown;
        targetX = homeX = transform.position.x;
    }

    void Update()
    {
        gravity = gameManager.Instance.gravity;
        currentState = state.getState();
        switch (state.getState())
        {
            case "Movement":
                handleMovement();
                break;
            case "Charge":
                handleCharge();
                break;
            case "Slide":
                handleSlide();
                break;
            case "Bonk":
                velocity.x *= 0.97f;
                velocity.y *= 0.99f;
                gravity = gameManager.Instance.gravity/1.5f;
                break;                
            default:
                break;
        }
        anim.SetBool("isGrounded", isGrounded());
        applyGravity();
    }

    // Update is called once per frame
    void handleMovement()
    {
        anim.SetBool("isFalling", !isGrounded() && (velocity.y < -1.0f));
        anim.SetBool("isWalking", isGrounded() && (Mathf.Abs(velocity.x) > 0));
        if (velocity.x != 0) setFacing((int)Mathf.Sign(velocity.x));
        
        if (lineOfSight() && isGrounded() && ((Time.time - lastCharge) > 1.0f))
        {
            triggerState("Snarl");
            resetVelocity();
            return;
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

    public void StartCharge()
    {
        chargeStart = Time.time;
        state.setState("Charge");
    }

    void handleCharge()
    {
        velocity.x = chargeVelocity * getFacing();

        if (Time.time - chargeStart > chargeDuration)
        {
            triggerState("Slide");
            chargeStart = Mathf.Infinity;
        }

        if ((controller.collisions.left && getFacing() == -1) ||
            (controller.collisions.right && getFacing() == 1))
        {
            wallBonk();
            triggerState("Bonk");
        }
    }

    protected override void OnReturnToMovement()
    {
        lastCharge = Time.time;
    }

    void wallBonk()
    {
        createStars(starOrigin.position);
        velocity.x = -3.0f * getFacing();
        velocity.y = 3.5f;
        returnToMovement(1.0f);
    }

    void handleSlide()
    {
        velocity.x *= 0.95f;
        returnToMovement(0.5f, true);
    }

    void applyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    void FixedUpdate()
    {
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
            if ((controller.collisions.above && velocity.y > 0) ||
                (controller.collisions.below && velocity.y < 0))
            {
                velocity.y = 0;
            }
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