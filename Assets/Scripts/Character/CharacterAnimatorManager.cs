using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterAnimatorManager : MonoBehaviour
{
    protected Animator animator;
    protected CharacterLocomotion character;

    private int horizontal;
    private int vertical;

    private int grounded;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        character = GetComponent<CharacterLocomotion>();

        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");

        grounded = Animator.StringToHash("IsGrounded");
    }

    protected virtual void Update()
    {
        animator.SetBool(grounded, character.isGrounded);
    }

    public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float horizontalAmount = horizontalMovement;
        float verticalAmount = verticalMovement;

        if (isSprinting) verticalAmount = 2;

        animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);
    }

    public virtual void PlayTargetActionAnimation(
        string targetAnimation, 
        bool isPerformingAction, 
        bool applyRootMotion = true, 
        bool canRotate = false, 
        bool canMove = false)
    {
        character.applyRootMotion = applyRootMotion;
        animator.CrossFade(targetAnimation, 0.2f);
        // Can be used to stop character from attempting new actions.
        // For example, if you get damaged and begin performing a damage animation
        // this flag will turn true if you are stunned.
        // We can check for this before attempting new actions.
        character.isPerformingAction = isPerformingAction;
        character.canRotate = canRotate;
        character.canMove = canMove;
    }
}
