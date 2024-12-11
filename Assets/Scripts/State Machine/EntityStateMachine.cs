using UnityEngine;
using System.Threading.Tasks;

/*
    We should define what an Entity is and
    possibly use that class name for Entities?

    All of the components we are assuming an 
    Entity has should be required. Check on OnValidate
    see if we can automatically generate child components
    if they dont exist. Or at least complain
*/

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(ColliderManager))]
[RequireComponent(typeof(HealthController))]
public abstract class EntityStateMachine : StateMachine
{

    /*
        Is this denormalized?
        I remember for the turn around animation we use it or 
        something
        If this is nessessary it should require a setter
    */
    [SerializeField] public int facing = 1;

    /*
        This is for switching animations per phase
        and should be removed
    */
    public string animClipPrefix { get; protected set; }

    [HideInInspector] public CharacterController controller;
    [HideInInspector] public ParticleMaker particleMaker;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Transform sprite;
    [HideInInspector] public ColliderManager colliderManager;
    [HideInInspector] public Deformer deformer;

    public override void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        deformer = GetComponentInChildren<Deformer>();
        particleMaker = GetComponentInChildren<ParticleMaker>();
        colliderManager = GetComponent<ColliderManager>();
        sprite = transform.Find("Sprite");
        base.Start();
    }

    /*
        Looks like we may have normalized facing and sprite 
        x-scale.
        This method should be the only way to modify the value
        of facing.
    */
    public void setFacing(float vel)
    {
        if (Mathf.Abs(vel) < 0.01f) return;
        if (AtlasHelpers.Sign(vel) == 0) return;

        int dir = AtlasHelpers.Sign(vel);
        if (dir != 0) facing = dir;

        sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x) * facing, sprite.localScale.y, sprite.localScale.z);
    }

    /*
        This is why Atlas was hitting herself. Not sure this
        should be here.
        We might want an EnemyStateMachine.
        At some point we'll need to rethink the Collision 
        system to make it inheritable (#868b9gqve)
    */
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "AllyHitbox")
        {
            HitBox hb = other.GetComponent<AllyHitBoxController>().hitbox;
            hurt(hb);
        }
    }

    public virtual void hurt(HitBox hitbox) { }
}