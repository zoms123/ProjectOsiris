using System;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.TextCore.Text;

public class InputMovementSystem : PlayerSystem
{
    [Header("References")]
    [SerializeField, Required] private InputManagerSO inputManager;

    [Header("Ground Check & Falling")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckSphereRadius = 0.3f;
    [Tooltip("The force at which the character is sticking to the ground whilst it is grounded")]
    [SerializeField] protected float groundedYVelocity = -20f;
    [Tooltip("The force at which the character begins to fall when it become ungrounded (Rises as it falls longer)")]
    [SerializeField] protected float fallStartYVelocity = -5f;
    [SerializeField] protected float gravityFactor = -9.81f;

    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 2f;
    [SerializeField] private float runningSpeed = 5f;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float jumpForwardSpeed = 5f;
    [SerializeField] private float airMovementSpeed = 2f;
    [SerializeField] private float freeFallSpeed = 2f;

    private Vector3 moveDirection;
    private Vector3 jumpDirection;
    private Vector3 verticalVelocity;
    private Vector2 inputDirection;

    private float jumpFactor;
    private float inAirTimer = 0;
    private float currentSpeed;
    private float moveAmount = 0f;

    private bool lockRotation = false;
    private bool fallingVelocityHasBeenSet = false;
    private bool isSprintInputPressed = false;
    private bool isPerformingAction = false;
    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isStrafing = false;
    private bool applyRootMotion = false;
    private bool canRotate = true;
    private bool canMove = true;

    private float JUMP_FACTOR_WALKING = 0.25f;
    private float JUMP_FACTOR_RUNNING = 0.5f;

    private Transform mainCameraTransform;
    [HideInInspector] public CharacterController characterController;

    protected override void Awake()
    {
        base.Awake();

        characterController = GetComponent<CharacterController>();
        mainCameraTransform = Camera.main.transform;
        jumpFactor = JUMP_FACTOR_WALKING;
    }

    #region Events

    private void OnEnable()
    {
        inputManager.OnMove += Move;
        inputManager.OnJump += Jump;

        player.ID.playerEvents.OnAnimationChanged += UpdateVariablesAffectedByAnimation;
        player.ID.playerEvents.OnUpdateMovementByAnimator += UpdateMovementByAnimator;

        player.ID.playerEvents.OnPlayerActiveSprint += ActiveSprintMode;
        player.ID.playerEvents.OnPlayerDesactiveSprint += DesactiveSprintMode;

        player.ID.playerEvents.OnPlayerAim += ActiveOrDesactiveAim;

        player.ID.playerEvents.OnLockRotation += LockRotation;
        player.ID.playerEvents.OnUnlockRotation += UnlockRotation;
    }

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

    private void UpdateVariablesAffectedByAnimation(
        bool isPerformingAction,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false)
    {
        this.isPerformingAction = isPerformingAction;
        this.applyRootMotion = applyRootMotion;
        this.canRotate = canRotate;
        this.canMove = canMove;
    }

    private void UpdateMovementByAnimator(Vector3 velocity, Quaternion rotation)
    {
        if (applyRootMotion)
        {
            characterController.Move(velocity);
            transform.rotation *= rotation;
        }
    }

    private void ActiveSprintMode(float newSpeed, float newJumpFactor)
    {
        if (!isStrafing) 
        {
            currentSpeed = newSpeed;
            jumpFactor = newJumpFactor;
            isSprintInputPressed = true;
        }
    }

    private void DesactiveSprintMode()
    {
        isSprintInputPressed = false;
    }

    private void ActiveOrDesactiveAim(bool isAiming)
    {
        isStrafing = isAiming;
    }

    private void LockRotation()
    {
        lockRotation = true;
        isStrafing = true;
    }

    private void UnlockRotation()
    {
        lockRotation = false;
        isStrafing = false;
    }

    private void OnDisable()
    {
        inputManager.OnMove -= Move;
        inputManager.OnJump -= Jump;

        player.ID.playerEvents.OnAnimationChanged -= UpdateVariablesAffectedByAnimation;
        player.ID.playerEvents.OnUpdateMovementByAnimator -= UpdateMovementByAnimator;

        player.ID.playerEvents.OnPlayerActiveSprint -= ActiveSprintMode;
        player.ID.playerEvents.OnPlayerDesactiveSprint -= DesactiveSprintMode;

        player.ID.playerEvents.OnPlayerAim -= ActiveOrDesactiveAim;

        player.ID.playerEvents.OnLockRotation -= LockRotation;
        player.ID.playerEvents.OnUnlockRotation -= UnlockRotation;
    }

    #endregion

    private void Update()
    {
        HandleGroundCheck();
        ManageVerticalSpeed();
        HandleMovementInput();

        HandleAllMovement();
    }

    #region Methods

    private void AttemptToPerformJump()
    {
        if (isPerformingAction || isJumping || !isGrounded) return;

        if (moveAmount < 0.1f)
            player.ID.playerEvents.OnChangeAnimation?.Invoke("Jump", false, true, true, false);
        else
            player.ID.playerEvents.OnChangeAnimation?.Invoke("JumpMove", false, true, true, false);

        isJumping = true;
        jumpDirection = GetMoveDirection();

        if (jumpDirection != Vector3.zero)
            jumpDirection *= jumpFactor;
    }

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

    private void HandleGroundCheck()
    {
        if (Physics.CheckSphere(transform.position, groundCheckSphereRadius, groundLayer) != isGrounded)
        {
            isGrounded = !isGrounded;
            player.ID.playerEvents.OnAnimationGroundedUpdate?.Invoke(isGrounded);
        }
    }

    private void ManageVerticalSpeed()
    {
        if (isGrounded)
        {
            // If we are not attempting to jump or move upward
            if (verticalVelocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                verticalVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            // If we are not jumping and falling velocity has not been set yet
            if (!isJumping && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                verticalVelocity.y = fallStartYVelocity;
            }

            inAirTimer += Time.deltaTime;

            verticalVelocity.y += gravityFactor * Time.deltaTime;
        }

        // There should always be some force applied to the Y velocity
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    private void HandleMovementInput()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(inputDirection.y) + Mathf.Abs(inputDirection.x));

        bool isSprinting = isSprintInputPressed && moveAmount >= 0.5f;

        if (moveAmount <= 0.5f && moveAmount > 0)
        {
            moveAmount = 0.5f;
            if (!isSprinting)
            {
                currentSpeed = walkingSpeed;
                jumpFactor = JUMP_FACTOR_WALKING;
            }
        }
        else if (moveAmount > 0.5f && moveAmount <= 1)
        {
            moveAmount = 1;
            if (!isSprinting)
            {
                currentSpeed = runningSpeed;
                jumpFactor = JUMP_FACTOR_RUNNING;
            }  
        }

        // If it is strafing, pass the horizontal movement as well (cannot run)
        if (isStrafing)
        {
            // Calculate relative movement direction when strafing
            Vector3 localMoveDirection = transform.InverseTransformDirection(moveDirection);
            player.ID.playerEvents.OnUpdateAnimationMovementParameters?.Invoke(localMoveDirection.x, localMoveDirection.z, false);
        }
        // If it is not strafing, only use the move amount (can run)
        else
        {
            player.ID.playerEvents.OnUpdateAnimationMovementParameters?.Invoke(0, moveAmount, isSprinting);
        }
    }

    private void HandleAllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandleFreeFallMovement();
    }

    private void HandleGroundedMovement()
    {
        if (!canMove) return;

        moveDirection = GetMoveDirection();

        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        if (lockRotation)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                                                  mainCameraTransform.rotation.eulerAngles.y,
                                                  transform.rotation.eulerAngles.z);
            isStrafing = true;
            return;
        }

        if (!canRotate || isStrafing) return;

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

            characterController.Move(freeFallSpeed * Time.deltaTime * freeFallDirection);
        }
    }

    private void HandleJumpingMovement()
    {
        if (isJumping)
        {
            if (jumpDirection != Vector3.zero) characterController.Move(jumpForwardSpeed * Time.deltaTime * jumpDirection);

            Vector3 airMoveDirection = mainCameraTransform.forward * inputDirection.y + mainCameraTransform.right * inputDirection.x;
            airMoveDirection.Normalize();
            airMoveDirection.y = 0;

            characterController.Move(airMovementSpeed * Time.deltaTime * airMoveDirection);
        }
    }

    #endregion

    #region Public Methods

    public void ApplyJumpingVelocity()
    {
        verticalVelocity.y = Mathf.Sqrt(-2 * jumpHeight * gravityFactor);
    }

    public void ResetActionFlags()
    {
        isPerformingAction = false;
        applyRootMotion = false;
        canRotate = true;
        canMove = true;
        isJumping = false;
    }

    public void ResetJump()
    {
        isJumping = false;
    }

    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, groundCheckSphereRadius);
    }

    #endregion
}
