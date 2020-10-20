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

    protected StateMachine state;
    public string currentState;
    //A List of States where the enemy cannot be hit by hitboxes
    protected List<State> intangibleStates;

    public float weight = 1.0f;
    public float visionRange = 5.0f;
    public float moveSpeed = 0;

    //Direction Enemy is Facing
    protected int facing = 1;
    //Keep track if enemy has seen player or not
    protected bool awakened = false;
    //Use to set the time between actions
    protected float actionCoolDown = 2.0f;
    protected float act = 0;

    //Physics vars
    public bool isGrounded = true;
    public Vector3 velocity;
    protected float gravity;
    protected float maxFallVel;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<characterController>();
        deformer = GetComponentInChildren<Deformer>();
        sprite = transform.Find("sprite");

        gravity = gameManager.Instance.gravity;
        maxFallVel = gameManager.Instance.maxFallVel;

        state = new StateMachine("Movement");
        state.addStates("Hurt", "Wait", "Attack");
        intangibleStates = state.findStates("Hurt", "Wait");
        state.setState("Movement");
    }

    protected virtual void FixedUpdate()
    {
        isGrounded = controller.collisions.below;
    }

    public void hurt(HitBox hitbox)
    {
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
            gameManager.Instance.createInstance("Effects/Fire/IncendiaryParticle", rpos + transform.position, transform);
        }

        deformer.flashWhite();

        StartCoroutine(getHurt());
    }

    protected bool lineOfSight()
    {
        int wallsLayer = LayerMask.NameToLayer("Wall");
        int playerLayer = LayerMask.NameToLayer("Player");

        Vector3 direction = transform.localScale;
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
        state.setState(statename);
        anim.SetTrigger(statename);
    }

    protected void returnToMovement()
    {
        state.setState("Movement");
        anim.SetTrigger("returnToMovement");
    }
}
