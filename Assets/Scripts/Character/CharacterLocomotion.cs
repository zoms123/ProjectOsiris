using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotion : CharacterManager
{
    [Header("Ground Check & Falling")]
    [SerializeField] protected float gravityFactor = -9.81f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckSphereRadius = 0.3f;
    [Tooltip("The force at which the character is sticking to the ground whilst it is grounded")]
    [SerializeField] protected float groundedYVelocity = -20f;
    [Tooltip("The force at which the character begins to fall when it become ungrounded (Rises as it falls longer)")]
    [SerializeField] protected float fallStartYVelocity = -5f;
    // The force at which our character is pulled up or down (Jumping or Falling)
    protected Vector3 velocity;
    protected bool fallingVelocityHasBeenSet = false;
    protected float inAirTimer = 0;

    protected virtual void Update()
    {
        HandleGroundCheck();

        if (isGrounded)
        {
            // If we are not attempting to jump or move upward
            if (velocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                velocity.y = groundedYVelocity;
            }
        }
        else
        {
            // If we are not jumping and falling velocity has not been set yet
            if (!isJumping && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                velocity.y = fallStartYVelocity;
            }

            inAirTimer += Time.deltaTime;

            velocity.y += gravityFactor * Time.deltaTime;
        }

        // There should always be some force applied to the Y velocity
        characterController.Move(velocity * Time.deltaTime);
    }

    protected void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckSphereRadius, groundLayer);
    }

    protected void OnDrawGizmos()
    {
        // Draws ground check sphere in scene view
        Gizmos.DrawWireSphere(transform.position, groundCheckSphereRadius);
    }
}
