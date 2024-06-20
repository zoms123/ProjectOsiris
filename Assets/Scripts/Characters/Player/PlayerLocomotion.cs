using System;
using UnityEngine;

public class PlayerLocomotion : CharacterLocomotion
{
    [Header("Movement Settings")]

    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float sprintingSpeed = 7f;
    [SerializeField] private float rotationSpeed = 15f;
    private Vector2 inputDirection;
    private Vector3 moveDirection;
    private float moveAmount = 0f;
    private bool isSprintInputPressed = false;

    [Header("Jump Settings")]

    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpForwardSpeed = 5f;
    [SerializeField] private float freeFallSpeed = 2f;
    private Vector3 jumpDirection;
    public bool lockRotation;

    [Header("Inputs")]

    [SerializeField, Required] private InputManagerSO inputManager;

    private PlayerAnimatorManager playerAnimatorManager;
    private Transform mainCameraTransform;

    protected override void Awake()
    {
        base.Awake();

        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        mainCameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        inputManager.OnMove += Move;
        inputManager.OnJump += Jump;
        inputManager.OnSprintPressed += Sprint;
        inputManager.OnSprintReleased += StopSprint;
    }

    // Only runs when the input is being updated
    private void Move(Vector2 movementInput)
    {
        inputDirection = movementInput;
    }

    private void Jump(bool isJumpPressed)
    {
        if (isJumpPressed)
        {
            AttemptToPerformJump();
        }
    }

    private void Sprint()
    {
        isSprintInputPressed = true;
    }

    private void StopSprint()
    {
        isSprintInputPressed = false;
    }

    protected override void Update()
    {
        base.Update();

        HandleMovementInput();
    }

    public void HandleAllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandleFreeFallMovement();
    }

    private void HandleMovementInput()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(inputDirection.y) + Mathf.Abs(inputDirection.x));

        if (moveAmount <= 0.5f && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5f && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        isSprinting = isSprintInputPressed && moveAmount >= 0.5f;

        // If it is strafing, pass the horizontal movement as well (cannot run)
        if (isStrafing)
        {
            Debug.Log($"Input direction: {inputDirection}");
            // Calculate relative movement direction when strafing
            Vector3 localMoveDirection = transform.InverseTransformDirection(moveDirection);
            playerAnimatorManager.UpdateAnimatorMovementParameters(localMoveDirection.x, localMoveDirection.z, false);
            //playerAnimatorManager.UpdateAnimatorMovementParameters(inputDirection.x, moveDirection.z, false);
        }
        // If it is not strafing, only use the move amount (can run)
        else
        {
            playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, isSprinting);
        }
    }

    // Movement direction is based on camera facing perspective and movement inputs
    private Vector3 GetMoveDirection()
    {
        Vector3 forward = mainCameraTransform.forward;
        Vector3 right = mainCameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * inputDirection.y + right * inputDirection.x;
        moveDir.y = 0f;
        moveDir.Normalize();

        return moveDir;
    }

    private void HandleGroundedMovement()
    {
        if (!canMove) return;

        moveDirection = GetMoveDirection();

        float currentSpeed = isSprinting && !isStrafing ? sprintingSpeed : (moveAmount > 0.5f ? runningSpeed : walkingSpeed);

        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (!canRotate || isStrafing) return;

        if (lockRotation)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                                                  mainCameraTransform.rotation.eulerAngles.y,
                                                  transform.rotation.eulerAngles.z);
            return;
        }

        Vector3 targetRotationDirection = GetMoveDirection();

        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    private void HandleFreeFallMovement()
    {
        if (!isGrounded && !isJumping)
        {
            Vector3 freeFallDirection = mainCameraTransform.forward * inputDirection.y + mainCameraTransform.right * inputDirection.x;
            freeFallDirection.Normalize();
            freeFallDirection.y = 0;

            characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
        }
    }

    private void HandleJumpingMovement()
    {
        if (isJumping)
        {
            characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }

    private void AttemptToPerformJump()
    {
        if (isPerformingAction || isJumping || !isGrounded) return;

        if (moveAmount < 0.1f)
            playerAnimatorManager.PlayTargetActionAnimation("Jump", false, true, true);
        else
            playerAnimatorManager.PlayTargetActionAnimation("JumpMove", false, true, true);

        isJumping = true;

        jumpDirection = GetMoveDirection();

        if (jumpDirection != Vector3.zero)
        {
            // If sprinting, jump direction is at full distance
            if (isSprinting)
            {
                jumpDirection *= 1;
            }
            // If running, jump direction is at half distance
            else if (moveAmount > 0.5f)
            {
                jumpDirection *= 0.5f;
            }
            // If walking, jump direction is at quarter distance
            else if (moveAmount <= 0.5f)
            {
                jumpDirection *= 0.25f;
            }
        }
    }

    public void ApplyJumpingVelocity()
    {
        velocity.y = Mathf.Sqrt(-2 * jumpHeight * gravityFactor);
    }

    private void OnDisable()
    {
        inputManager.OnMove -= Move;
        inputManager.OnJump -= Jump;
        inputManager.OnSprintPressed -= Sprint;
        inputManager.OnSprintReleased -= StopSprint;
    }
}
