using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimationSystem : PlayerSystem
{
    private Animator animator;

    [Header("Animation Settings")]
    [SerializeField, Required] private Rig bodyRig;
    [SerializeField] private float movementAnimationDampTime = 0.2f;
    [SerializeField] private float transitionSpeed = 10f;

    private float layerWeight = 0f;
    private float layerTargetWeight = 0f;

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

        bodyRig.weight = 0f;
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

    private void UpdateAimParameters(int layerIndex, float weight, float targetWeight)
    {
        layerWeight = weight;
        layerTargetWeight = targetWeight;

        // apply upper body layer and body rig weights
        bodyRig.weight = Mathf.Lerp(layerWeight, layerTargetWeight, Time.deltaTime * transitionSpeed);

        animator.SetLayerWeight(layerIndex, layerWeight);
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
