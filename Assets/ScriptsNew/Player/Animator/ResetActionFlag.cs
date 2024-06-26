using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetActionFlag : StateMachineBehaviour
{
    InputMovementSystem movementSystem;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (movementSystem == null)
        {
            movementSystem = animator.GetComponent<InputMovementSystem>();
        }

        // This is called when an action ends, and the state returns to "Empty"

        movementSystem.ResetActionFlags();
    }
}
