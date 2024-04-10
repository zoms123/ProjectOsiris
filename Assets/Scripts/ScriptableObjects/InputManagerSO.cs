using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Managers/InputManager")]
public class InputManagerSO : ScriptableObject
{
    public Controls controls;

    public event Action<bool> OnJump;
    public event Action<Vector2> OnMove;
    public event Action<bool> OnSprint;
    public event Action OnAttack;
    public event Action OnLockTarget;
    public event Action OnOptions;
    public event Action<Vector2> OnPowerSelect;
    public event Action OnFire;
    public event Action OnInteract;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new();

            controls.PlayerMovement.Move.performed += Move;
            controls.PlayerMovement.Move.canceled += Move;
            controls.PlayerActions.Jump.performed += Jump;
            controls.PlayerActions.Sprint.performed += Sprint;
            controls.PlayerActions.Sprint.canceled += Sprint;
            controls.Gameplay.Attack.started += Attack;
            controls.Gameplay.LockTarget.started += LockTarget;
            controls.Gameplay.Options.started += Options;
            controls.Gameplay.PowerSelect.started += PowerSelect;
            controls.Gameplay.Fire.started += Fire;
            controls.Gameplay.Interact.started += Interact;
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

    private void Sprint(InputAction.CallbackContext context)
    {
        OnSprint?.Invoke(context.ReadValueAsButton());
    }

    private void Attack(InputAction.CallbackContext context)
    {
        OnAttack?.Invoke();
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

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.PlayerMovement.Move.performed -= Move;
            controls.PlayerMovement.Move.canceled -= Move;
            controls.PlayerActions.Jump.started -= Jump;
            controls.PlayerActions.Sprint.performed -= Sprint;
            controls.PlayerActions.Sprint.canceled -= Sprint;
            controls.Gameplay.Attack.started -= Attack;
            controls.Gameplay.LockTarget.started -= LockTarget;
            controls.Gameplay.Options.started -= Options;
            controls.Gameplay.PowerSelect.started -= PowerSelect;

            controls.Gameplay.Disable();
            controls.PlayerMovement.Disable();
            controls.PlayerActions.Disable();
            Debug.Log("Input Disabled!");
        }
    }
}
