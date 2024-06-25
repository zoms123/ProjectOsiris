using UnityEngine;

public class InputSprintSystem : PlayerSystem
{
    [Header("References")]
    [SerializeField, Required] private InputManagerSO inputManager;

    [Header("Sprint Settings")]
    [SerializeField] private float sprintingSpeed = 7f;

    private float JUMP_FACTOR_SPRINTING = 1f;

    #region Events

    private void OnEnable()
    {
        inputManager.OnSprintPressed += Sprint;
        inputManager.OnSprintReleased += StopSprint;
    }

    private void Sprint()
    {
        player.ID.playerEvents.OnPlayerActiveSprint?.Invoke(sprintingSpeed,JUMP_FACTOR_SPRINTING);
    }

    private void StopSprint()
    {
        player.ID.playerEvents.OnPlayerDesactiveSprint?.Invoke();
    }

    private void OnDisable()
    {
        inputManager.OnSprintPressed -= Sprint;
        inputManager.OnSprintReleased -= StopSprint;
    }

    #endregion
}
