using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EntityStateMachine
{
    private float weight = 1.0f;
    private float visionRange = 5.0f;
    private float moveSpeed = 0;

    //Hitbox hurts other enemies
    private bool harmEnemies = false;

    //Keep track if enemy has seen player or not
    public bool awakened;
    public bool playerSpotted;

    //Use to set the time between actions
    protected float actionCoolDown = 2.0f;
    [SerializeField] protected float act;

    bool returningToMovement = false;

    public override void Start()
    {
        act = actionCoolDown;
        base.Start();
    }
    public override void Update()
    {
        if (awakened) {
            if (canAct()) {
                act = 0;
            } else {
                act += Time.deltaTime;
            }
        }
        base.Update();
    }

    public bool canAct() {
        return act >= actionCoolDown;
    }

    public void FaceTowardsPlayer() {
        Vector3 playerPosition = gameManager.Instance.player.transform.position;
        float dx = playerPosition.x - transform.position.x;
        setFacing(AtlasHelpers.Sign(dx));
    }

    public void hurt(HitBox hitbox)
    {
        // if (!hitbox) return;
        // float kbStrength = (hitbox.knockback ? 2.5f : 1.5f) / weight;
        // float dx = hitbox.kbDir.x;
        // float dy = hitbox.kbDir.y;

        // if (hitbox.explosive)
        // {
        //     Vector2 dir = (transform.position - hitbox.position).normalized;
        //     dx = dir.x;
        //     dy = dir.y;
        // }
        // controller.velocity.x = kbStrength * dx;
        // controller.velocity.y = kbStrength * 1.5f * dy;
        // act = Mathf.Max(actionCoolDown / 2.0f, act);
        // anim.SetBool("Hurt", true);

        // if (hitbox.incendiary)
        // {
        //     Vector2 size = GetComponent<BoxCollider2D>().size/2.0f;
        //     Vector3 rpos = new Vector3(Random.Range(-size.x, size.x), Random.Range(-size.y, size.y), 0);
        //     gameManager.createInstance("Effects/Fire/IncendiaryParticle", rpos + transform.position, transform);
        // }

        // deformer.flashColor();

        // StartCoroutine(getHurt());
    }

    protected bool lineOfSight()
    {
        int wallsLayer = LayerMask.NameToLayer("Wall");
        int playerLayer = LayerMask.NameToLayer("Player");

        Vector3 direction = new Vector3(transform.localScale.x, 0, 0);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, visionRange, (1 << wallsLayer) | (1 << playerLayer));

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    protected void returnToMovement(float waitSeconds = 0, bool resetVel = false)
    {
        if (!returningToMovement) StartCoroutine(myfunc());

        IEnumerator myfunc()
        {
            returningToMovement = true;

            yield return new WaitForSeconds(waitSeconds); 
            if (resetVel) controller.velocity = Vector2.zero;
            OnReturnToMovement();

            returningToMovement = false;
        }
    }

    protected virtual void OnReturnToMovement()
    {
    }

    protected void createStars(Vector3 position)
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/Effects/StarParticles"), position, Quaternion.identity);
    }
//TODO shouldn't need
    protected void resetAnimator()
    {
        // foreach (AnimatorControllerParameter parameter in anim.parameters)
        // {
        //     if (parameter.name == "resetSpin") continue;
        //     if (parameter.type == AnimatorControllerParameterType.Bool)
        //     {
        //         anim.SetBool(parameter.name, false);
        //     }
        //     if (parameter.type == AnimatorControllerParameterType.Trigger)
        //     {
        //         anim.ResetTrigger(parameter.name);
        //     }
        // }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (harmEnemies && other.CompareTag("Enemy")) {
            HealthController hc = other.GetComponent<HealthController>();
            hc.takeDamage(3.0f);
        }
    }
}
