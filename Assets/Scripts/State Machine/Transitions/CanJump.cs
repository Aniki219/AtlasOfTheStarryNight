using UnityEngine;
using System;
using System.Threading.Tasks;

namespace Transitions {
    public class CanJump : IStateTransition {
        playerController pc;

        public override void attach(State state) {
            base.attach(state);
            pc = (playerController)state.stateMachine;    
        }

        public override void checkCondition() {
            if (AtlasInputManager.getKeyPressed("Jump")) {
                switch (state) {
                    case States.Move move:
                    case States.Slip slip:
                        groundJump();
                        break;
                    case States.Jump jump:
                    case States.Fall fall:
                    case States.Broom broom:
                        doubleJump();
                        break;
                    case States.WallSlide wallSlide:
                        wallJump();
                        break;
                    default:
                        groundJump();
                        break;
                }        
            }
        }

        private void groundJump() {
            changeState(new States.Jump());
        }

        private void doubleJump() {
            if (pc.canDoubleJump) {
                changeState(new States.DoubleJump());
                state.anim.SetBool("isDoubleJumping", true);
            }
        }

        private void wallJump() {
            // state.anim.SetBool("wallBlast", true);
            // changeState(new States.WallJump);
        }

    }
}