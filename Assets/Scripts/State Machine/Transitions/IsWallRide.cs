using UnityEngine;
using System;
using States;
using SuperTiled2Unity.Editor.LibTessDotNet;
using Behaviors;
using System.Data;

namespace Transitions {
    public class IsWallRide : IStateTransition {
        protected bool switchOnIsWall;

        public IsWallRide() {
            switchOnIsWall = true;
        }

        public override void checkCondition() {
            Vector2 castDirection = -state.sprite.transform.up;
            RaycastHit2D hit = Physics2D.Raycast(state.transform.position, 
                                                castDirection, 
                                                state.colliderManager.getCollider().size.y + 0.1f, 
                                                state.controller.collisionMask);

            //TODO: Maybe we have to make sure the collider is tangential to castDirection?
            if (hit.collider == switchOnIsWall) {
                state.skipExitState = true;
                ChangeState();
            }
        }

        protected virtual void ChangeState() {
            changeState(new WallRide(state.GetBehavior<BroomBehavior>()));
        }
    }
    
    public class IsNotWallRide : IsWallRide {
        public IsNotWallRide() : base() {
            switchOnIsWall = false;
        }

        protected override void ChangeState() {
            changeState(new Broom(state.GetBehavior<BroomBehavior>()).SkipStartAnim());
        }
    }
}
