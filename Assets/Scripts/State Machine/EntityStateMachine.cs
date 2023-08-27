using UnityEngine;
using System.Threading.Tasks;

[RequireComponent(typeof(CharacterController))] 
[RequireComponent(typeof(ColliderManager))] 
[RequireComponent(typeof(HealthController))]
public abstract class EntityStateMachine : StateMachine {
  [SerializeField] public int facing = 1;
  public string animClipPrefix {get; protected set;}

  [HideInInspector] public CharacterController controller;
  [HideInInspector] public ParticleMaker particleMaker;
  [HideInInspector] public Animator anim;
  [HideInInspector] public Transform sprite;
  [HideInInspector] public ColliderManager colliderManager;
  [HideInInspector] public Deformer deformer;

  public override void Start() {
    controller = GetComponent<CharacterController>();
    anim = GetComponentInChildren<Animator>();
    deformer = GetComponentInChildren<Deformer>();
    particleMaker = GetComponentInChildren<ParticleMaker>();
    colliderManager = GetComponent<ColliderManager>();
    sprite = transform.Find("Sprite");
    base.Start();
  }

  public void setFacing(float vel) {
    if (Mathf.Abs(vel) < 0.01f) return;
    if (AtlasHelpers.Sign(vel) == 0) return;

    int dir = AtlasHelpers.Sign(vel);
    if (dir != 0) facing = dir;

    sprite.localScale = new Vector3(Mathf.Abs(sprite.localScale.x) * facing, sprite.localScale.y, sprite.localScale.z);
  }

  protected virtual void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "AllyHitbox")
    {
      HitBox hb = other.GetComponent<AllyHitBoxController>().hitbox;
      hurt(hb);
    }
  }

  public virtual void hurt(HitBox hitbox) {}
}