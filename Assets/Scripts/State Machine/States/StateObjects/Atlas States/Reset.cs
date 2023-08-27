using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace States {
  public class Reset : AtlasState {
    float resetTime = 0.3f;
    Vector3 velocitySmoothing;

    GameObject starRotator;

    int defaultSortingOrder;

    public Reset() {
      behaviors = new List<IStateBehavior>() {

      };

      transitions = new List<IStateTransition>() {

      };
    }

    public async override Task StartState()
    {
      await base.StartState();
      defaultSortingOrder = sprite.GetComponent<SpriteRenderer>().sortingOrder;
      starRotator = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/StarRotator"), transform);
      controller.collisions.setTangible(false);
    }

    public override void UpdateState()
    {
      base.UpdateState();
      colliderManager.disableCollider("Secret");
        sprite.GetComponent<SpriteRenderer>().sortingOrder = 25;

        if (Vector3.SqrMagnitude(pc.lastSafePosition - transform.position) < 0.01f)
        {
            transform.position = pc.lastSafePosition;
            controller.velocity = Vector3.zero;
            pc.changeState<Idle>();
        }
        transform.position = Vector3.SmoothDamp(transform.position, pc.lastSafePosition, ref velocitySmoothing, resetTime);
    
    }

    public async override Task ExitState()
    {
      GameObject.Destroy(starRotator);
      colliderManager.enableCollider("Secret");
      sprite.GetComponent<SpriteRenderer>().sortingOrder = defaultSortingOrder;
      controller.collisions.setTangible(true);
      await base.ExitState();
    }
  }
}