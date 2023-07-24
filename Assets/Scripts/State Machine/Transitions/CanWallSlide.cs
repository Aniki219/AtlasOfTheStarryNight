using UnityEngine;
using System;

namespace Transitions {
    public class CanWallSlide : IStateTransition {
        private characterController cc;
        private Collider2D collider;

        public override void attach(State state) {
            base.attach(state);
            cc = state.controller;
            collider = state.colliderManager.getCollider();
        }

        public override void checkCondition() {
            float directionX = AtlasInputManager.getAxis("Dpad").getValue().x;
            Vector2 midRay = cc.collisions.getMidPoint() - (Vector2)collider.bounds.center;
            Vector2 footRay = cc.collisions.getFootPoint() - ((Vector2)collider.bounds.min + collider.bounds.extents.x * Vector2.right);

            if (directionX != 0 &&
                cc.collisions.hasNormWhere(norm => Vector2.Dot(directionX * Vector2.right, norm) == -1) &&
                Mathf.Abs(Vector2.Dot(midRay.normalized, footRay.normalized)) >= 0.8f) {
                    changeState(new States.WallSlide());
                }
        }
    }

    public class CanUnwallSlide : CanWallSlide {
        private characterController cc;
        private Collider2D collider;

        public override void attach(State state) {
            base.attach(state);
            cc = state.controller;
            collider = collider = state.colliderManager.getCollider();
        }

        public override void checkCondition() {
            float directionX = AtlasInputManager.getAxis("Dpad").getValue().x;
            Vector2 midRay = cc.collisions.getMidPoint() - (Vector2)collider.bounds.center;
            Vector2 footRay = cc.collisions.getFootPoint() - ((Vector2)collider.bounds.min + collider.bounds.extents.x * Vector2.right);

            if (!(directionX != 0 &&
                cc.collisions.hasNormWhere(norm => Vector2.Dot(directionX * Vector2.right, norm) == -1) &&
                Mathf.Abs(Vector2.Dot(midRay.normalized, footRay.normalized)) >= 0.8f)) {
                    changeState(new States.Fall());
                }
        }
    }
}