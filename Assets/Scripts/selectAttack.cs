using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selectAttack : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController pc = animator.gameObject.GetComponentInParent<playerController>();
        animator.SetBool("Attacking", true);
        bool up = AtlasInputManager.getAxisState("Dpad").y > 0.1f;
        bool down = AtlasInputManager.getAxisState("Dpad").y < -0.1f;

        bool falling = (pc.velocity.y < -0.1);

        if (up) animator.SetTrigger("UpAttack");
        else if (down) animator.SetTrigger("DownAttack");
        else
        {
            if (!pc.isGrounded())
            {
                if (falling) {
                    animator.SetTrigger("FallingAttack");
                } else
                {
                    animator.SetTrigger("RisingAttack");
                }

            } else
            {
                animator.SetTrigger("Attack");
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
