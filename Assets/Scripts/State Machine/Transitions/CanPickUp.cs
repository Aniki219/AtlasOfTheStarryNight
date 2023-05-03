using UnityEngine;

namespace Transitions {
  public class CanPickUp : IStateTransition {
    public override void checkCondition() {
      if (AtlasInputManager.getKeyPressed("Down") && state.controller.isGrounded())
      {
          RaycastHit2D pickup = Physics2D.Raycast(state.transform.position, -Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Pickupable"));
          if (pickup.collider != null)
          {
              pickup.transform.SendMessage("pickUp");
              state.stateMachine.changeState(new States.Carry());
          }
      }
    }
  }
}