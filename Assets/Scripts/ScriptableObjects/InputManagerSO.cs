using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[CreateAssetMenu(menuName = "Managers/InputManager")]
public class InputManagerSO : ScriptableObject
{
    public Controls controls;

    public event Action<bool> OnJump;
    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLook;
    public event Action OnSprintPressed;
    public event Action OnSprintReleased;
    public event Action OnAttack;
    public event Action OnInteract;
    public event Action<bool> OnAim;
    public event Action OnOptions;
    public event Action<Vector2> OnPowerSelect;
    public event Action OnCombatAbility;
    public event Action OnPuzzleAbility;

    public event Action<Vector2> OnControlObjectXY;
    public event Action<Vector2> OnControlObjectZ;

    public CursorLockMode cursorLockMode = CursorLockMode.None;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new();

            controls.PlayerMovement.Move.performed += Move;
            controls.PlayerMovement.Move.canceled += Move;

            controls.PlayerCamera.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
            controls.PlayerCamera.Look.canceled += ctx => Look(Vector2.zero);

            controls.PlayerActions.Jump.performed += Jump;

            controls.PlayerActions.Sprint.performed += x => Sprint();
            controls.PlayerActions.SprintFinish.performed += x => SprintCancel();

            controls.Gameplay.Attack.started += Attack;

            controls.Gameplay.Interact.started += Interact;

            controls.Gameplay.Aim.performed += ctx => Aim(ctx.ReadValueAsButton());
            controls.Gameplay.Aim.canceled += ctx => Aim(ctx.ReadValueAsButton());

            controls.Gameplay.Options.started += Options;

            controls.Gameplay.PowerSelect.started += PowerSelect;

            controls.Gameplay.CombatAbility.started += CombatAbility;

            controls.Gameplay.PuzzleAbility.started += PuzzleAbility;
        }

        controls.Enable();
        Debug.Log("Input ready!");
    }

    private void Move(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>());
    }

    private void Look(Vector2 newLookDirection)
    {
        OnLook?.Invoke(newLookDirection);
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

    private void Aim(bool newAimState)
    {
        OnAim?.Invoke(newAimState);
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

        controls.PlayerGravityPuzzle.ControlObjectXY.performed += ControlObjectX;
        controls.PlayerGravityPuzzle.ControlObjectXY.canceled += ControlObjectX;
        controls.PlayerGravityPuzzle.ControlObjectZ.performed += ControlObjectY;
        controls.PlayerGravityPuzzle.ControlObjectZ.canceled += ControlObjectY;

        controls.PlayerGravityPuzzle.Enable();
    }

    public void PuzzleGravityAbilityDisabled()
    {
        controls.Gameplay.PowerSelect.started += PowerSelect;
        controls.Gameplay.CombatAbility.started += CombatAbility;

        controls.PlayerGravityPuzzle.ControlObjectXY.performed -= ControlObjectX;
        controls.PlayerGravityPuzzle.ControlObjectXY.canceled -= ControlObjectX;
        controls.PlayerGravityPuzzle.ControlObjectZ.performed -= ControlObjectY;
        controls.PlayerGravityPuzzle.ControlObjectZ.canceled -= ControlObjectY;

        controls.PlayerGravityPuzzle.Disable();
    }

    private void ControlObjectX(InputAction.CallbackContext context)
    {
        OnControlObjectXY?.Invoke(context.ReadValue<Vector2>());
    }

    private void ControlObjectY(InputAction.CallbackContext context)
    {
        OnControlObjectZ?.Invoke(context.ReadValue<Vector2>());
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Disable();
            Debug.Log("Input Disabled!");

            //PuzzleGravityAbilityDisabled();
        }
    }

    public void SetCursorState(CursorLockMode cursorMode)
    {
        cursorLockMode = cursorMode;
        Cursor.lockState = cursorLockMode;
    }
}
