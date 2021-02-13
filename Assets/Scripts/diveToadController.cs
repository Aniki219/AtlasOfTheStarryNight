using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class diveToadController : MonoBehaviour
{
    characterController controller;
    Animator anim;
    Vector3 velocity;
    Deformer deformer;
    int facing;

    float gravity;
    float maxFallVel;

    public float actionCoolDown = 2.0f;
    healthController health;
    public float act = 0;
    bool awakened = false;

    public bool readyToPounce = false;

    public State state;

    public enum State
    {
        Wait,
        Movement,
        Idle,
        Hurt,
        Attack,
        Jump
    }

    //Any state that cannot receive damage or interact with triggers
    List<State> intangibleStates = new List<State>
    {
        State.Hurt
    };

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<healthController>();
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<characterController>();
        deformer = GetComponentInChildren<Deformer>();

        facing = (int)transform.localScale.x;
        setFacing();
        gravity = gameManager.Instance.gravity;
        maxFallVel = gameManager.Instance.maxFallVel;
        state = State.Movement;
        act = (readyToPounce) ? 0 : actionCoolDown;
    }

    void Update()
    {
        if (!gameManager.Instance.player) return;
        switch (state)
        {
            case State.Movement:
                handleMovement();
                applyGravity();
                break;
            case State.Hurt:
                applyGravity();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void handleMovement()
    {
        anim.SetBool("Jumping", !isGrounded() && (velocity.y > 0));
        anim.SetBool("Falling", !isGrounded() && (velocity.y < -0.5f));

        float dx = gameManager.Instance.player.transform.position.x - transform.position.x;
        float dy = transform.position.y - gameManager.Instance.player.transform.position.y;

        float xdist = Mathf.Abs(dx);
        bool playerSpotted = (xdist <= 3.0f && dy > -1f && dy < 5f);

        if (playerSpotted) {
            facing = (int)Mathf.Sign(dx);
        }

        //If the player is spotted
        if (playerSpotted || awakened)
        {
            setFacing();
            //wait to act
            if (act > 0)
            {
                act -= Time.deltaTime;
            }
            else
            {
                //If they are within jump range
                if (xdist <= 2.5f)
                {
                    act = actionCoolDown;
                    //If they are below
                    if (dy > 1)
                    {
                        jump();
                    }
                    else
                    {
                        attack();
                    }
                    awakened = true;
                } else if (awakened)
                {
                    //If the player is out of range but the toad has already jumped before
                    //it will hop around randomly
                    if (Random.value > 0.5f)
                    {
                        facing *= -1;
                    } else
                    {
                        jump();
                    }
                    act = actionCoolDown * 1.5f;
                }
            }
        }
        if (velocity.y < maxFallVel) { velocity.y = maxFallVel; }
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

    public void hurt(HitBox hitbox)
    {
        float kbStrength = (hitbox.knockback ? 2.5f : 1.5f);
        float dx = hitbox.kbDir.x;
        float dy = hitbox.kbDir.y;

        if (hitbox.explosive)
        {
            Vector2 dir = (transform.position - hitbox.position).normalized;
            dx = dir.x;
            dy = dir.y;
        }
        velocity.x = kbStrength * dx;
        velocity.y = kbStrength * 1.5f * dy;
        act = Mathf.Max(actionCoolDown / 2.0f, act);
        anim.SetBool("Hurt", true);

        StartCoroutine(getHurt());
    }

    IEnumerator getHurt()
    {
        state = State.Hurt;
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Hurt", false);
        anim.SetTrigger("Idle");
        state = State.Movement;
    }

    void setFacing()
    {
        if (!isGrounded()) return;
        transform.localScale = new Vector3(facing * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void FixedUpdate()
    {
        controller.Move(velocity * Time.deltaTime);
        if (controller.collisions.above || controller.collisions.below)
        {
            if (anim.GetBool("Falling"))
            {
                if (velocity.y <= -4f)
                {
                    anim.SetTrigger("Land");
                }
                else
                {
                    if (velocity.y < -1f)
                    {
                        deformer.startDeform(new Vector3(1.2f, 0.75f, 1.0f), 0.1f, 0.1f);
                    }
                    anim.SetTrigger("Idle");
                }
            }
            if (velocity.y < 0)
            {
                velocity.x = 0;
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

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.tag == "AllyHitbox")
    //    {
    //        HitBox hb = other.GetComponent<AllyHitBoxController>().hitbox;
    //        hurt(hb, false);
    //    }
    //}

    public bool isGrounded()
    {
        return controller.collisions.below;
    }
}
