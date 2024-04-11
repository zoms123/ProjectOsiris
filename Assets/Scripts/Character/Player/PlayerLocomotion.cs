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

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpForwardSpeed = 5f;
    [SerializeField] private float freeFallSpeed = 2f;
    private Vector3 jumpDirection;

    #region Components
    [Header("Components")]
    [SerializeField] private InputManagerSO inputManager;
    private PlayerAnimatorManager playerAnimatorManager;
    private Transform mainCameraTransform;
    #endregion

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
        inputManager.OnSprint += Sprint;
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

    private void Sprint(bool isSprintPressed)
    {
        isSprinting = isSprintPressed && moveAmount >= 0.5;
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

        // If it is locked on, pass the horizontal movement as well (cannot run)
        if (isStrafing)
        {
            playerAnimatorManager.UpdateAnimatorMovementParameters(moveDirection.x, moveDirection.z, false);
        }
        // If it is not locked on, only use the move amount (can run)
        else
        {
            playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, isSprinting);
        }

    }

    private void HandleGroundedMovement()
    {
        if (!canMove) return;

        // Movement direction is based on camera facing perspective and movement inputs
        moveDirection = mainCameraTransform.forward * inputDirection.y + mainCameraTransform.right * inputDirection.x;
        moveDirection.Normalize();
        moveDirection.y = 0f;

        if (isSprinting && !isStrafing)
        {
            // Move at sprinting speed
            characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        else
        {
            if (moveAmount > 0.5f)
            {
                // Move at running speed
                characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (moveAmount <= 0.5f)
            {
                // Move at walking speed
                characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleRotation()
    {
        if (!canRotate || isStrafing) return;

        Vector3 targetRotationDirection = mainCameraTransform.forward * inputDirection.y + mainCameraTransform.right * inputDirection.x;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0f;

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

        jumpDirection = mainCameraTransform.forward * inputDirection.y + mainCameraTransform.right * inputDirection.x;
        jumpDirection.Normalize();
        jumpDirection.y = 0;

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
        inputManager.OnSprint -= Sprint;
    }
}
