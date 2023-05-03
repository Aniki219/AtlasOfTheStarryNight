using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(characterController), typeof(Animator), typeof(Deformer))]
public abstract class enemyAI : MonoBehaviour
{
    //chracterController contains physics
    protected characterController controller;
    protected Animator anim;
    protected Deformer deformer;
    protected Transform sprite;
    protected BoxCollider2D boxCollider;
    protected healthController healthCtrl;

    protected DeprecatedStateMachine state;
    public string currentState;
    //A List of States where the enemy cannot be hit by hitboxes
    protected List<DeprecatedState> intangibleStates;

    public float weight = 1.0f;
    public float visionRange = 5.0f;
    public float moveSpeed = 0;

    //Hitbox hurts toher enemies
    protected bool harmEnemies = false;

    //Direction Enemy is Facing
    protected int facing = 1;
    //Keep track if enemy has seen player or not
    protected bool awakened = false;
    //Use to set the time between actions
    protected float actionCoolDown = 2.0f;
    protected float act = 0;

    //Physics vars
    public Vector3 velocity;
    protected float gravity;
    protected float maxFallVel;

    bool returningToMovement = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<characterController>();
        deformer = GetComponentInChildren<Deformer>();
        boxCollider = GetComponent<BoxCollider2D>();
        healthCtrl = GetComponent<healthController>();
        sprite = transform.Find("sprite");

        // gravity = gameManager.Instance.gravity;
        // maxFallVel = gameManager.Instance.maxFallVel;

        state = new DeprecatedStateMachine("Movement");
        state.addStates("Hurt", "Wait", "Attack");
        intangibleStates = state.findStates("Hurt", "Wait");
        state.setState("Movement");
    }

    public void hurt(HitBox hitbox)
    {
        if (!hitbox) return;
        float kbStrength = (hitbox.knockback ? 2.5f : 1.5f) / weight;
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

        if (hitbox.incendiary)
        {
            Vector2 size = GetComponent<BoxCollider2D>().size/2.0f;
            Vector3 rpos = new Vector3(Random.Range(-size.x, size.x), Random.Range(-size.y, size.y), 0);
            gameManager.createInstance("Effects/Fire/IncendiaryParticle", rpos + transform.position, transform);
        }

        deformer.flashColor();

        StartCoroutine(getHurt());
    }

    protected bool lineOfSight()
    {
        int wallsLayer = LayerMask.NameToLayer("Wall");
        int playerLayer = LayerMask.NameToLayer("Player");

        Vector3 direction = new Vector3(transform.localScale.x, 0, 0);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, visionRange, (1 << wallsLayer) | (1 << playerLayer));

        return (hit.collider != null && hit.collider.CompareTag("Player"));
    }

    protected virtual IEnumerator getHurt()
    {
        state.setState("Hurt");
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Hurt", false);
        anim.SetTrigger("returnToMovement");
        state.setState("Movement");
    }

    protected void triggerState(string statename)
    {
        resetAnimator();
        state.setState(statename);
        anim.SetTrigger(statename);
    }

    protected void returnToMovement(float waitSeconds = 0, bool resetVel = false)
    {
        if (!returningToMovement) StartCoroutine(myfunc());

        IEnumerator myfunc()
        {
            returningToMovement = true;

            yield return new WaitForSeconds(waitSeconds); 
            state.setState("Movement");
            anim.SetTrigger("returnToMovement");
            if (resetVel) resetVelocity();
            OnReturnToMovement();

            returningToMovement = false;
        }
    }

    protected virtual void OnReturnToMovement()
    {
    }

    protected void setFacing(int facing = 1)
    {
        if (!isGrounded()) return;
        transform.localScale = new Vector3(facing * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    protected float getFacing()
    {
        return transform.localScale.x;
    }

    protected void resetVelocity()
    {
        velocity = Vector3.zero;
    }

    protected bool isGrounded()
    {
        return controller.collisions.getBelow();
    }

    protected void createStars(Vector3 position)
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/Effects/StarParticles"), position, Quaternion.identity);
    }

    protected void resetAnimator()
    {
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            if (parameter.name == "resetSpin") continue;
            if (parameter.type == AnimatorControllerParameterType.Bool)
            {
                anim.SetBool(parameter.name, false);
            }
            if (parameter.type == AnimatorControllerParameterType.Trigger)
            {
                anim.ResetTrigger(parameter.name);
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (harmEnemies && other.CompareTag("Enemy")) {
            healthController hc = other.GetComponent<healthController>();
            hc.takeDamage(3.0f);
        }
    }
}
