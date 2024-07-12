using System;
using UnityEngine;

public struct PlayerEvents
{
    // Movement Events
    public Action<bool, bool, bool, bool> OnAnimationChanged;
    public Action<Vector3, Quaternion> OnUpdateMovementByAnimator;

    // Sprint Events
    public Action<float, float> OnPlayerActiveSprint;
    public Action OnPlayerDesactiveSprint;

    // Aiming Events
    public Action<bool> OnPlayerAim;
    public Action<Vector3, bool> OnAimPositionReceived;
    public Action<bool, float, float> OnUpdateCameraSettings;

    // Powers Events
    public Action OnGetAimPosition;
    public Action OnLockRotation;
    public Action OnUnlockRotation;
    public Action<float, float> OnChangeCameraRange;
    public Action OnResetCameraRange;
    public Action OnUseCombatAbility;
    public Action<Vector3> OnFirePower;

    // Animation Events
    public Action<string, bool, bool, bool, bool> OnChangeAnimation;
    public Action<bool> OnAnimationGroundedUpdate;
    public Action<float, float, bool> OnUpdateAnimationMovementParameters;
    public Action<int, float, float> OnUpdateAimParameters;

    // Audio Events
    public Action<AudioClip> OnPlayerPlaySound;
}
