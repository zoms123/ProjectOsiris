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
    public event Action OnLockTarget;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Controls();

            controls.Gameplay.Move.performed += Move;
            controls.Gameplay.Move.canceled += Move;
            controls.Gameplay.Jump.started += Jump;
            controls.Gameplay.Run.performed += Run;
            controls.Gameplay.Run.canceled += Run;
            controls.Gameplay.LockTarget.started += LockTarget;
        }

        controls.Gameplay.Enable();
        Debug.Log("Input ready!");
    }

    private void Move(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>());
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

    private void LockTarget(InputAction.CallbackContext context)
    {
        OnLockTarget?.Invoke();
    }
}
