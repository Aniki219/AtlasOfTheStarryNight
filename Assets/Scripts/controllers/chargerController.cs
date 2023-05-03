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

    ParticleSystem dustTrail;
    float rollStartTime = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        state.addStates("Snarl", "Charge", "Slide", "Plop", "Bonk", "Pop", "Roll");
        intangibleStates.AddRange(state.findStates("Plop", "Bonk"));

        actionCoolDown = 1.5f;
        act = (readyToPounce) ? 0 : actionCoolDown;
        targetX = homeX = transform.position.x;

        setFacing((int)transform.localScale.x);

        dustTrail = sprite.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        gravity = gameManager.Instance.gravity;
        handleDustParticles();
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
                act = actionCoolDown;
                break;
            case "Roll":
                handleRoll();
                break;
            default:
                break;
        }
        anim.SetBool("isGrounded", isGrounded());
        applyGravity();

        if (state.getState() == "Roll") {
            healthCtrl.blocking = true;
            harmEnemies = true;
        } else
        {
            healthCtrl.blocking = false;
            harmEnemies = false;
            sprite.rotation = Quaternion.Euler(0, 0, 0);
        }
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

            float dir = Mathf.Sign(targetX - transform.position.x);
            float dist = Mathf.Abs(targetX - transform.position.x);
            if (dist > moveSpeed * 2.0f * Time.deltaTime)
            {
                velocity.x = moveSpeed * dir;
            }
            else
            {
                velocity.x = 0;
            }
        } else
        {
            act -= Time.deltaTime;
        }

        if (velocity.y < maxFallVel) { velocity.y = maxFallVel; }
    }

    void handleDustParticles()
    {
        var emission = dustTrail.emission;
        emission.rateOverDistance = 0;
        if (state.getState() == "Charge") emission.rateOverDistance = 1.5f;
        if (state.getState() == "Slide") emission.rateOverDistance = 10f;

        var shape = dustTrail.shape;
        shape.position = new Vector3(-0.5f * getFacing(), -0.2f, 0);
        shape.scale = sprite.localScale;
    }

    void handleRoll()
    {
        float rspeed = -getFacing() * velocity.x * Time.deltaTime * 180.0f/Mathf.PI;
        sprite.Rotate(new Vector3(0, 0, rspeed));
        velocity.x = Mathf.Lerp(velocity.x, 0, 1*Time.deltaTime);
        if (Time.time - rollStartTime > 3.0f)
        {
            endRoll();
        }
    }

    public void StartCharge()
    {
        chargeStart = Time.time;
        state.setState("Charge");
    }

    void handleCharge()
    {
        velocity.x = chargeVelocity * getFacing();

        if (Time.time - chargeStart > chargeDuration && !lineOfSight())
        {
            triggerState("Slide");
            chargeStart = Mathf.Infinity;
        }

        if ((controller.collisions.getLeft() && getFacing() == -1) ||
            (controller.collisions.getRight() && getFacing() == 1))
        {
            wallBonk();
            triggerState("Bonk");
        }
    }

    protected override void OnReturnToMovement()
    {
        lastCharge = Time.time;
        targetX = homeX = transform.position.x;
    }

    void wallBonk()
    {
        createStars(transform.position + 0.25f * Vector3.right);
        deformer.startDeform(new Vector3(0.5f, 1.5f, 1.0f), 0.1f, 0.5f, default, "bonk", true);
        velocity.x = -3.0f * getFacing();
        velocity.y = 3.5f;
        returnToMovement(1.0f, true);
    }

    void handleSlide()
    {
        velocity.x *= 0.95f;
        float slideDuration = 0.75f;
        returnToMovement(slideDuration, true);
        act = actionCoolDown;
    }

    void applyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    void FixedUpdate()
    {
        controller.Move(velocity * Time.deltaTime);
        if (controller.collisions.getAbove() || controller.collisions.getBelow())
        {
            if (anim.GetBool("isFalling"))
            {
                deformer.startDeform(new Vector3(1.2f, 0.75f, 1.0f), 0.1f, 0.1f);
                if (velocity.y <= -4f)
                {
                    triggerState("Plop");
                    resetVelocity();
                    returnToMovement(1.5f, true);
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
            if ((controller.collisions.getAbove() && velocity.y > 0) ||
                (controller.collisions.getBelow() && velocity.y < 0))
            {
                velocity.y = 0;
            }
        }
        if ((controller.collisions.getRight() && velocity.x > 0) || (controller.collisions.getLeft() && velocity.x < 0))
        {
            velocity.x = 0;
            if (velocity.y > 0)
            {
                velocity.y = 0;
            }
        }
    }

    public void startRoll()
    {
        velocity.x = healthCtrl.lastHitBy.kbDir.x * 4.0f;
        velocity.y = 4.0f;
        resetAnimator();
        triggerState("Roll");
        sprite.localPosition = new Vector3(0, -0.1f, 0);
        rollStartTime = Time.time;
    }

    void endRoll()
    {
        velocity.y = 3.5f;
        sprite.localPosition = new Vector3(0, 0, 0);
        sprite.localRotation = Quaternion.Euler(0,0,0);
        anim.SetTrigger("Pop");
        deformer.startDeform(new Vector3(1.5f, 1.5f, 1.5f), 0.1f, 0.25f, default, "regrow", true);
        returnToMovement(0.25f, true);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }
}