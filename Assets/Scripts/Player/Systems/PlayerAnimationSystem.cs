using System;
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
    private int combatAbility;

    public bool isUsingCombatAbility = false;
    private Vector3 aimPosition;
    private bool isAiming;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();

        AssignAnimationIDs();

        bodyRig.weight = 0f;
    }

    #region Events

    private void OnEnable()
    {
        player.ID.playerEvents.OnChangeAnimation += PlayTargetActionAnimation;
        player.ID.playerEvents.OnUpdateAnimationMovementParameters += UpdateAnimatorMovementParameters;
        player.ID.playerEvents.OnAnimationGroundedUpdate += UpdateGrounded;
        player.ID.playerEvents.OnUpdateAimParameters += UpdateAimParameters;
        player.ID.playerEvents.OnAimPositionReceived += GetAimPosition;
        player.ID.playerEvents.OnUseCombatAbility += TriggerCombatAbilityAnimation;
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

    private void GetAimPosition(Vector3 aimPosition, bool isAiming)
    {
        this.aimPosition = aimPosition;
        this.isAiming = isAiming;
    }

    private void TriggerCombatAbilityAnimation()
    {
        if (aimPosition != null)
        {
            if (!isUsingCombatAbility && isAiming)
            {
                isUsingCombatAbility = true;
                animator.SetTrigger(combatAbility);
            }
        }
        else Debug.Log($"Aim position is invalid, Combat Ability animation not triggered");
    }

    // Combat Ability animation event handler
    private void OnCombatAbilityAnimationEvent() => player.ID.playerEvents.OnFirePower.Invoke(aimPosition);

    // Combat Ability animation event handler
    private void ResetCombatAbility() => isUsingCombatAbility = false;

    private void AssignAnimationIDs()
    {
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
        grounded = Animator.StringToHash("IsGrounded");
        combatAbility = Animator.StringToHash("CombatAbility");
    }

    private void OnDisable()
    {
        player.ID.playerEvents.OnChangeAnimation -= PlayTargetActionAnimation;
        player.ID.playerEvents.OnUpdateAnimationMovementParameters -= UpdateAnimatorMovementParameters;
        player.ID.playerEvents.OnAnimationGroundedUpdate -= UpdateGrounded;
        player.ID.playerEvents.OnUpdateAimParameters -= UpdateAimParameters;
        player.ID.playerEvents.OnUseCombatAbility -= TriggerCombatAbilityAnimation;
        player.ID.playerEvents.OnAimPositionReceived -= GetAimPosition;
    }

    #endregion

    private void OnAnimatorMove()
    {
        player.ID.playerEvents.OnUpdateMovementByAnimator?.Invoke(animator.deltaPosition, animator.deltaRotation);
    }
}
