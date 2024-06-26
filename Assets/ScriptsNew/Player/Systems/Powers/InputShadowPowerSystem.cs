using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class InputShadowPowerSystem : PlayerSystem
{
    [Header("References")]
    [SerializeField, Required] private InputManagerSO inputManager;
    [SerializeField, Required] private GameManagerSO gameManager;

    [Header("Shadow Power Settings")]
    [SerializeField] private float shadowCombatAbilityCooldown;

    private float shadowWaitTime = 0;

    private bool shadowIsActive;

    #region Events

    private void OnEnable()
    {
        inputManager.OnPowerSelect += OnPowerSelected;
        inputManager.OnCombatAbility += CombatAbility;
        inputManager.OnPuzzleAbility += PuzzleAbility;
    }

    private void OnPowerSelected(Vector2 power)
    {
        if (power[1] == 1)
        {
            shadowIsActive = true;
            gameManager.PlayerChangePower(PowerType.Shadow);
        }
        else
        {
            shadowIsActive = false;
        }
    }

    private void CombatAbility()
    {
        if (shadowIsActive && shadowWaitTime <= 0)
        {
            shadowWaitTime = shadowCombatAbilityCooldown;
        }
    }

    private void PuzzleAbility()
    {
        if (shadowIsActive)
        {

        }
    }

    private void OnDisable()
    {
        if (shadowIsActive)
        {
            shadowIsActive = false;
            gameManager.PlayerChangePower(PowerType.None);
        }

        inputManager.OnPowerSelect -= OnPowerSelected;
        inputManager.OnCombatAbility -= CombatAbility;
        inputManager.OnPuzzleAbility -= PuzzleAbility;
    }

    #endregion

    private void Update()
    {
        shadowWaitTime -= Time.deltaTime;
    }
}
