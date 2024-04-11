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
    private PlayerManager player;
    private Transform mainCameraTransform;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
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
        player.isSprinting = isSprintPressed && moveAmount >= 0.5;
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

        if (player == null) return;

        // If it is locked on, pass the horizontal movement as well (cannot run)
        if (player.isStrafing)
        {
            player.animatorManager.UpdateAnimatorMovementParameters(moveDirection.x, moveDirection.z, false);
        }
        // If it is not locked on, only use the move amount (can run)
        else
        {
            player.animatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.isSprinting);
        }

    }

    private void HandleGroundedMovement()
    {
        if (!player.canMove) return;

        // Movement direction is based on camera facing perspective and movement inputs
        moveDirection = mainCameraTransform.forward * inputDirection.y + mainCameraTransform.right * inputDirection.x;
        moveDirection.Normalize();
        moveDirection.y = 0f;

        if (player.isSprinting && !player.isStrafing)
        {
            // Move at sprinting speed
            player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        else
        {
            if (moveAmount > 0.5f)
            {
                // Move at running speed
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (moveAmount <= 0.5f)
            {
                // Move at walking speed
                player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleRotation()
    {
        if (!player.canRotate || player.isStrafing) return;

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
        if (!player.isGrounded)
        {
            Vector3 freeFallDirection = mainCameraTransform.forward * inputDirection.y + mainCameraTransform.right * inputDirection.x;
            freeFallDirection.Normalize();
            freeFallDirection.y = 0;

            player.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
        }
    }

    private void HandleJumpingMovement()
    {
        if (player.isJumping)
        {
            player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }

    private void AttemptToPerformJump()
    {
        if (player.isPerformingAction || player.isJumping || !player.isGrounded) return;

        if (moveAmount < 0.1f)
            player.animatorManager.PlayTargetActionAnimation("Jump", false, true, true);
        else
            player.animatorManager.PlayTargetActionAnimation("JumpMove", false, true, true);

        player.isJumping = true;

        jumpDirection = mainCameraTransform.forward * inputDirection.y + mainCameraTransform.right * inputDirection.x;
        jumpDirection.Normalize();
        jumpDirection.y = 0;

        if (jumpDirection != Vector3.zero)
        {
            // If sprinting, jump direction is at full distance
            if (player.isSprinting)
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
