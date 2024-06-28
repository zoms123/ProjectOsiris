using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Managers/InputManager")]
public class InputManagerSO : ScriptableObject
{
    public Controls controls;

    public event Action<bool> OnJump;
    public event Action<Vector2> OnMove;
    public event Action<Vector2> OnLook;
    public event Action OnSprintPressed;
    public event Action OnSprintReleased;
    public event Action OnInteract;
    public event Action<bool> OnAim;
    public event Action OnOptions;
    public event Action<Vector2> OnPowerSelect;
    public event Action OnCombatAbility;
    public event Action OnPuzzleAbility;

    public event Action<Vector2> OnControlObjectXY;
    public event Action<Vector2> OnControlObjectZ;

    public CursorLockMode cursorLockMode = CursorLockMode.None;

    private bool puzzleGravityAbilityIsEnabled = false;

    #region Events

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new();

            EnableGameplayInputs();
            EnableUI();
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
            DisableGameplayInputs();
            DisableUI();

            controls.Disable();
            Debug.Log("Input Disabled!");
        }
    }

    #endregion

    #region Enable and Disable Input Maps

    #region Gameplay Input Maps

    #region Player Movement Input Map

    private void EnablePlayerMovement()
    {
        controls.PlayerMovement.Move.performed += Move;
        controls.PlayerMovement.Move.canceled += Move;

        controls.PlayerMovement.Jump.performed += Jump;

        controls.PlayerMovement.Sprint.performed += x => Sprint();
        controls.PlayerMovement.SprintFinish.performed += x => SprintCancel();

        controls.PlayerMovement.Enable();
    }

    private void DisablePlayerMovement()
    {
        controls.PlayerMovement.Move.performed -= Move;
        controls.PlayerMovement.Move.canceled -= Move;

        controls.PlayerMovement.Jump.performed -= Jump;

        controls.PlayerMovement.Sprint.performed -= x => Sprint();
        controls.PlayerMovement.SprintFinish.performed -= x => SprintCancel();

        controls.PlayerMovement.Disable();
    }

    #endregion

    #region Player Camera Input Map

    private void EnablePlayerCamera()
    {
        controls.PlayerCamera.Look.performed += ctx => Look(ctx.ReadValue<Vector2>());
        controls.PlayerCamera.Look.canceled += ctx => Look(Vector2.zero);

        controls.PlayerCamera.Enable();
    }

    private void DisablePlayerCamera()
    {
        controls.PlayerCamera.Look.performed -= ctx => Look(ctx.ReadValue<Vector2>());
        controls.PlayerCamera.Look.canceled -= ctx => Look(Vector2.zero);

        controls.PlayerCamera.Disable();
    }

    #endregion

    #region Player Actions Input Map

    private void EnablePlayerActions()
    {
        controls.PlayerActions.Interact.started += Interact;

        controls.PlayerActions.Aim.performed += ctx => Aim(ctx.ReadValueAsButton());
        controls.PlayerActions.Aim.canceled += ctx => Aim(ctx.ReadValueAsButton());

        controls.PlayerActions.Enable();
    }

    private void DisablePlayerActions()
    {
        controls.PlayerActions.Interact.started -= Interact;

        controls.PlayerActions.Aim.performed -= ctx => Aim(ctx.ReadValueAsButton());
        controls.PlayerActions.Aim.canceled -= ctx => Aim(ctx.ReadValueAsButton());

        controls.PlayerActions.Disable();
    }

    #endregion

    #region Player Powers Input Map

    private void EnablePlayerPowers()
    {
        controls.PlayerPowers.PowerSelect.started += PowerSelect;

        controls.PlayerPowers.CombatAbility.started += CombatAbility;

        controls.PlayerPowers.PuzzleAbility.started += PuzzleAbility;

        controls.PlayerPowers.Enable();
    }

    private void DisablePlayerPowers()
    {
        controls.PlayerPowers.PowerSelect.started -= PowerSelect;

        controls.PlayerPowers.CombatAbility.started -= CombatAbility;

        controls.PlayerPowers.PuzzleAbility.started -= PuzzleAbility;

        controls.PlayerPowers.Disable();
    }

    #endregion

    #region Player Gravity Puzzle Input Map

    private void EnablePlayerGravityPuzzle()
    {
        controls.PlayerGravityPuzzle.ControlObjectXY.performed += ControlObjectX;
        controls.PlayerGravityPuzzle.ControlObjectXY.canceled += ControlObjectX;

        controls.PlayerGravityPuzzle.ControlObjectZ.performed += ControlObjectY;
        controls.PlayerGravityPuzzle.ControlObjectZ.canceled += ControlObjectY;

        controls.PlayerGravityPuzzle.Enable();
    }

    private void DisablePlayerGravityPuzzle()
    {
        controls.PlayerGravityPuzzle.ControlObjectXY.performed -= ControlObjectX;
        controls.PlayerGravityPuzzle.ControlObjectXY.canceled -= ControlObjectX;

        controls.PlayerGravityPuzzle.ControlObjectZ.performed -= ControlObjectY;
        controls.PlayerGravityPuzzle.ControlObjectZ.canceled -= ControlObjectY;

        controls.PlayerGravityPuzzle.Disable();
    }

    #endregion

    public void EnableGameplayInputs()
    {
        EnablePlayerMovement();
        EnablePlayerCamera();
        EnablePlayerActions();

        if (puzzleGravityAbilityIsEnabled)
            EnablePlayerGravityPuzzle();
        else
            EnablePlayerPowers();
    }

    public void DisableGameplayInputs()
    {
        DisablePlayerMovement();
        DisablePlayerCamera();
        DisablePlayerActions();

        if (puzzleGravityAbilityIsEnabled)
            DisablePlayerGravityPuzzle();
        else
            DisablePlayerPowers();
    }

    #endregion

    #region UI Input Map

    public void EnableUI()
    {
        controls.UI.Options.started += Options;

        controls.UI.Enable();
    }

    public void DisableUI()
    {
        controls.UI.Options.started -= Options;

        controls.UI.Disable();
    }

    #endregion

    #endregion

    #region Public Methods

    public void PuzzleGravityAbilityEnabled()
    {
        DisablePlayerPowers();
        EnablePlayerGravityPuzzle();

        puzzleGravityAbilityIsEnabled = true;
    }

    public void PuzzleGravityAbilityDisabled()
    {
        DisablePlayerGravityPuzzle();
        EnablePlayerPowers();

        puzzleGravityAbilityIsEnabled = false;
    }

    public void SetCursorState(CursorLockMode cursorMode)
    {
        cursorLockMode = cursorMode;
        Cursor.lockState = cursorLockMode;
    }

    #endregion
}
