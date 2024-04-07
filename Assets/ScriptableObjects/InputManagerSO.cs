using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "InputManager")]
public class InputManagerSO : ScriptableObject
{
    private Controls controls;
    public event Action OnJump;
    public event Action<Vector2> OnMove;
    public event Action<bool> OnRun;
    public event Action OnAttack;
    public event Action OnLockTarget;
    public event Action OnOptions;
    public event Action<Vector2> OnPowerSelect;
    public event Action OnFire;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new();

            controls.Gameplay.Move.performed += Move;
            controls.Gameplay.Move.canceled += Move;
            controls.Gameplay.Jump.started += Jump;
            controls.Gameplay.Run.performed += Run;
            controls.Gameplay.Run.canceled += Run;
            controls.Gameplay.Attack.started += Attack;
            controls.Gameplay.LockTarget.started += LockTarget;
            controls.Gameplay.Options.started += Options;
            controls.Gameplay.PowerSelect.started += PowerSelect;
            controls.Gameplay.Fire.performed += Fire;
        }

        controls.Gameplay.Enable();
        Debug.Log("Input ready!");
    }

    private void OnDisable()
    {
        if (controls == null)
        {

            controls.Gameplay.Move.performed -= Move;
            controls.Gameplay.Move.canceled -= Move;
            controls.Gameplay.Jump.started -= Jump;
            controls.Gameplay.Run.performed -= Run;
            controls.Gameplay.Run.canceled -= Run;
            controls.Gameplay.Attack.started -= Attack;
            controls.Gameplay.LockTarget.started -= LockTarget;
            controls.Gameplay.Options.started -= Options;
            controls.Gameplay.PowerSelect.started -= PowerSelect;

            controls.Gameplay.Disable();
            Debug.Log("Input Disabled!");
        }
    }

    private void Move(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>());
    }

    private void Jump(InputAction.CallbackContext context)
    {
        OnJump?.Invoke();
    }

    private void Run(InputAction.CallbackContext context)
    {
        OnRun?.Invoke(context.ReadValueAsButton());
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

    private void Fire(InputAction.CallbackContext context)
    {
        OnFire?.Invoke();
    }
}
