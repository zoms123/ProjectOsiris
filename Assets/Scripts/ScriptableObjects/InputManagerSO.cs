using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Managers/InputManager")]
public class InputManagerSO : ScriptableObject
{
    public Controls controls;

    public event Action<bool> OnJump;
    public event Action<Vector2> OnMove;
    public event Action OnSprintPressed;
    public event Action OnSprintReleased;
    public event Action OnAttack;
    public event Action OnInteract;
    public event Action OnLockTarget;
    public event Action OnOptions;
    public event Action<Vector2> OnPowerSelect;
    public event Action OnCombatAbility;
    public event Action OnPuzzleAbility;

    public event Action<Vector2> OnControlObject;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new();

            controls.PlayerMovement.Move.performed += Move;
            controls.PlayerMovement.Move.canceled += Move;
            controls.PlayerActions.Jump.performed += Jump;
            controls.PlayerActions.Sprint.performed += x => Sprint();
            controls.PlayerActions.SprintFinish.performed += x => SprintCancel();
            controls.Gameplay.Attack.started += Attack;
            controls.Gameplay.Interact.started += Interact;
            controls.Gameplay.LockTarget.performed += LockTarget;
            controls.Gameplay.Options.started += Options;
            controls.Gameplay.PowerSelect.started += PowerSelect;
            controls.Gameplay.CombatAbility.started += CombatAbility;
            controls.Gameplay.PuzzleAbility.started += PuzzleAbility;
        }

        controls.Gameplay.Enable();
        controls.PlayerMovement.Enable();
        controls.PlayerActions.Enable();
        Debug.Log("Input ready!");
    }

    private void Move(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>());
    }

    private void Jump(InputAction.CallbackContext context)
    {
        OnJump?.Invoke(context.ReadValueAsButton());
    }

    private void Sprint()
    {
        OnSprintPressed?.Invoke();
    }

    private void SprintCancel()
    {
        OnSprintReleased?.Invoke();
    }

    private void Attack(InputAction.CallbackContext context)
    {
        OnAttack?.Invoke();
    }

    private void Interact(InputAction.CallbackContext context)
    {
        OnInteract?.Invoke();
    }

    private void LockTarget(InputAction.CallbackContext context)
    {
        OnLockTarget?.Invoke();
    }

    private void Options(InputAction.CallbackContext context)
    {
        OnOptions?.Invoke();
    }

    private void PowerSelect(InputAction.CallbackContext context)
    {
        OnPowerSelect?.Invoke(context.ReadValue<Vector2>());
    }

    private void CombatAbility(InputAction.CallbackContext context)
    {
        OnCombatAbility?.Invoke();
    }

    private void PuzzleAbility(InputAction.CallbackContext context)
    {
        OnPuzzleAbility?.Invoke();
    }

    public void PuzzleGravityAbilityEnabled()
    {
        controls.Gameplay.PowerSelect.started -= PowerSelect;
        controls.Gameplay.CombatAbility.started -= CombatAbility;

        controls.PlayerGravityPuzzle.ControlObject.performed += ControlObject;
        controls.PlayerGravityPuzzle.ControlObject.canceled += ControlObject;

        controls.PlayerGravityPuzzle.Enable();
    }

    public void PuzzleGravityAbilityDisabled()
    {
        controls.Gameplay.PowerSelect.started += PowerSelect;
        controls.Gameplay.CombatAbility.started += CombatAbility;

        controls.PlayerGravityPuzzle.ControlObject.performed -= ControlObject;
        controls.PlayerGravityPuzzle.ControlObject.canceled -= ControlObject;

        controls.PlayerGravityPuzzle.Disable();
    }

    private void ControlObject(InputAction.CallbackContext context)
    {
        OnControlObject?.Invoke(context.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.PlayerMovement.Move.performed -= Move;
            controls.PlayerMovement.Move.canceled -= Move;
            controls.PlayerActions.Jump.started -= Jump;
            controls.PlayerActions.Sprint.performed -= x => Sprint();
            controls.PlayerActions.SprintFinish.performed -= x => SprintCancel();
            controls.Gameplay.Attack.started -= Attack;
            controls.Gameplay.Interact.started -= Interact;
            controls.Gameplay.LockTarget.started -= LockTarget;
            controls.Gameplay.Options.started -= Options;
            controls.Gameplay.PowerSelect.started -= PowerSelect;
            controls.Gameplay.CombatAbility.started -= CombatAbility;
            controls.Gameplay.PuzzleAbility.started -= PuzzleAbility;

            controls.Gameplay.Disable();
            controls.PlayerMovement.Disable();
            controls.PlayerActions.Disable();
            Debug.Log("Input Disabled!");
        }
    }
}
