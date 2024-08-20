using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class InputMovementSystem : PlayerSystem
{
    [Header("References")]

    [SerializeField, Required]
    private InputManagerSO inputManager;
    private GameManagerSO gameManager;

    [Header("Ground Check & Falling")]

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private float groundCheckSphereRadius = 0.3f;

    [SerializeField, Tooltip("The force at which the character is sticking to the ground whilst it is grounded")]
    protected float groundedYVelocity = -20f;

    [SerializeField, Tooltip("The force at which the character begins to fall when it become ungrounded (Rises as it falls longer)")]
    protected float fallStartYVelocity = -5f;

    [SerializeField]
    protected float gravityFactor = -9.81f;

    [SerializeField, Tooltip("Distance threshold to ground")]
    private float groundCheckDistanceThreshold = 0.25f;

    [SerializeField, Tooltip("Transform to check if ground is being hit in front or in the back, based on player movement")]
    private Transform groundInfrontOrBackChecker;

    [SerializeField, Tooltip("Distance to check if ground is being hit in front or in the back, based on groundInfrontOrBackChecker")]
    private float groundInfrontOrBackCheckerDistanceThreshold = 2f;

    [SerializeField, Tooltip("Speed used when colliding with vertical terrain, allowing the player to be moved in the opposite direction of the wall")]
    private float verticalTerrainSlidingBackwardsSpeed = 3f;

    [Header("Movement Settings")]

    [SerializeField]
    private float walkingSpeed = 2f;

    [SerializeField]
    private float runningSpeed = 5f;

    [SerializeField]
    private float rotationSpeed = 15f;

    [Header("Jump Settings")]

    [SerializeField]
    private float jumpHeight = 3f;

    [SerializeField]
    private float jumpForwardSpeedWalking = 3f;

    [SerializeField]
    private float jumpForwardSpeedRunning = 6f;

    [SerializeField]
    private float airMovementSpeed = 2f;

    [SerializeField]
    private float freeFallSpeed = 2f;

    private float jumpForwardSpeed;

    private Vector3 moveDirection;
    private Vector3 jumpDirection;
    private Vector3 verticalVelocity;
    private Vector2 inputDirection;

    private float jumpFactor;
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

    private readonly float JUMP_FACTOR_WALKING = 0.25f;
    private readonly float JUMP_FACTOR_RUNNING = 0.5f;

    private Transform mainCameraTransform;

    [HideInInspector]
    public CharacterController characterController;

    private Vector3 groundContactPointOnAir;

    protected override void Awake()
    {
        base.Awake();

        characterController = GetComponent<CharacterController>();
        mainCameraTransform = Camera.main.transform;
        jumpFactor = JUMP_FACTOR_WALKING;
        characterController.slopeLimit = 45f;
    }

    #region Events

    private void OnEnable()
    {
        inputManager.OnMove += Move;
        inputManager.OnJump += Jump;
        inputManager.OnOptions += ResetMovement;

        player.ID.playerEvents.OnAnimationChanged += UpdateVariablesAffectedByAnimation;
        player.ID.playerEvents.OnUpdateMovementByAnimator += UpdateMovementByAnimator;

        player.ID.playerEvents.OnPlayerSprint += ActiveOrDesactiveSprintMode;

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

    private void ResetMovement()
    {
        if(Time.timeScale == 0)
        {
            ActiveOrDesactiveSprintMode(false, 0, 0);
            ActiveOrDesactiveAim(false);
            Move(Vector2.zero);
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

    private void ActiveOrDesactiveSprintMode(bool isSprintPressed, float newSpeed, float newJumpFactor)
    {
        if (isSprintPressed && !isStrafing)
        {
            currentSpeed = newSpeed;
            jumpFactor = newJumpFactor;
            isSprintInputPressed = true;
        }
        else
        {
            isSprintInputPressed = false;
        }
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
        inputManager.OnOptions -= ResetMovement;

        player.ID.playerEvents.OnAnimationChanged -= UpdateVariablesAffectedByAnimation;
        player.ID.playerEvents.OnUpdateMovementByAnimator -= UpdateMovementByAnimator;

        player.ID.playerEvents.OnPlayerSprint -= ActiveOrDesactiveSprintMode;

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

        jumpForwardSpeed = isSprintInputPressed ? jumpForwardSpeedRunning : jumpForwardSpeedWalking;

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
        bool previousIsGrounded = isGrounded;

        bool sphereCheck = Physics.CheckSphere(transform.position, groundCheckSphereRadius, groundLayer);

        if (!sphereCheck)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistanceThreshold, groundLayer))
            {
                isGrounded = true;
                Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.green);
            }
            else
            {
                isGrounded = false;
                Debug.DrawRay(transform.position, Vector3.down * groundCheckDistanceThreshold, Color.red);
            }
        }
        else
        {
            isGrounded = true;
            Debug.DrawRay(transform.position, Vector3.down * groundCheckDistanceThreshold, Color.green);
        }

        if (previousIsGrounded != isGrounded)
            player.ID.playerEvents.OnAnimationGroundedUpdate?.Invoke(isGrounded);
    }

    private void ManageVerticalSpeed()
    {
        if (isGrounded)
        {
            // If we are not attempting to jump or move upward
            if (verticalVelocity.y < 0)
            {
                fallingVelocityHasBeenSet = false;
                verticalVelocity = Vector3.zero;
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

            verticalVelocity.y += gravityFactor * Time.deltaTime;
            verticalVelocity += GetSlidingVelocity();

        }

        // There should always be some force applied to the Y velocity
        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    private Vector3 GetSlidingVelocity()
    {
        if (groundContactPointOnAir != Vector3.zero)
        {
            var moveDirection = groundContactPointOnAir - groundInfrontOrBackChecker.position;

            if (Physics.Raycast(groundInfrontOrBackChecker.position, moveDirection, out RaycastHit hit, groundInfrontOrBackCheckerDistanceThreshold, groundLayer))
            {
                Debug.DrawRay(groundInfrontOrBackChecker.position, moveDirection * hit.distance, Color.green);
                return moveDirection * -1 * verticalTerrainSlidingBackwardsSpeed;
            }
        }       

        return Vector3.zero;
    }

    private void HandleMovementInput()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(inputDirection.y) + Mathf.Abs(inputDirection.x));

        if (moveAmount <= 0.5f && moveAmount > 0)
        {
            moveAmount = 0.5f;
            if (!isSprintInputPressed)
            {
                currentSpeed = walkingSpeed;
                jumpFactor = JUMP_FACTOR_WALKING;
            }
        }
        else if (moveAmount > 0.5f && moveAmount <= 1)
        {
            moveAmount = 1;
            if (!isSprintInputPressed)
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
            bool isSprinting = isSprintInputPressed && moveAmount >= 0.5f;
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

        characterController.Move(currentSpeed * Time.deltaTime * moveDirection);
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

    #region Collisions

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if((groundLayer.value & (1 << hit.gameObject.layer)) != 0)
        {
            if (isGrounded)
                groundContactPointOnAir = Vector3.zero;
            else
                groundContactPointOnAir = hit.point;
        }
    }

    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, groundCheckSphereRadius);
    }

    #endregion
}
