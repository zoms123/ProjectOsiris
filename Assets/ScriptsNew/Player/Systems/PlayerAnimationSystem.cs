using UnityEngine;

public class PlayerAnimationSystem : PlayerSystem
{
    private Animator animator;

    [Header("Animation Settings")]
    [SerializeField] private float movementAnimationDampTime = 0.2f;

    private int horizontal;
    private int vertical;
    private int grounded;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();

        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
        grounded = Animator.StringToHash("IsGrounded");
    }

    #region Events

    private void OnEnable()
    {
        player.ID.playerEvents.OnChangeAnimation += PlayTargetActionAnimation;
        player.ID.playerEvents.OnUpdateAnimationMovementParameters += UpdateAnimatorMovementParameters;
        player.ID.playerEvents.OnAnimationGroundedUpdate += UpdateGrounded;
        player.ID.playerEvents.OnUpdateAimParameters += UpdateAimParameters;
    }

    private void PlayTargetActionAnimation(
        string targetAnimation,
        bool isPerformingAction,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false)
    {
        animator.CrossFade(targetAnimation, movementAnimationDampTime);
        player.ID.playerEvents.OnAnimationChanged?.Invoke(isPerformingAction, applyRootMotion, canRotate, canMove);
    }

    private void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float horizontalAmount = horizontalMovement;
        float verticalAmount = verticalMovement;

        if (isSprinting && verticalAmount >= 0.5f) verticalAmount = 2;

        animator.SetFloat(horizontal, horizontalAmount, movementAnimationDampTime, Time.deltaTime);
        animator.SetFloat(vertical, verticalAmount, movementAnimationDampTime, Time.deltaTime);
    }

    private void UpdateGrounded(bool isGrounded)
    {
        animator.SetBool(grounded, isGrounded);
    }

    private void UpdateAimParameters(int layerIndex, float weight)
    {
        animator.SetLayerWeight(layerIndex, weight);
    }

    private void OnDisable()
    {
        player.ID.playerEvents.OnChangeAnimation -= PlayTargetActionAnimation;
        player.ID.playerEvents.OnUpdateAnimationMovementParameters -= UpdateAnimatorMovementParameters;
        player.ID.playerEvents.OnAnimationGroundedUpdate -= UpdateGrounded;
        player.ID.playerEvents.OnUpdateAimParameters -= UpdateAimParameters;
    }

    #endregion

    private void OnAnimatorMove()
    {
        player.ID.playerEvents.OnUpdateMovementByAnimator?.Invoke(animator.deltaPosition, animator.deltaRotation);
    }
}
