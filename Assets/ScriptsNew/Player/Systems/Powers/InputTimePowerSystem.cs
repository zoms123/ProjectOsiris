using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class InputTimePowerSystem : PlayerSystem
{
    [Header("References")]
    [SerializeField, Required] private InputManagerSO inputManager;
    [SerializeField, Required] private GameManagerSO gameManager;

    [Header("Time Power Settings")]
    [SerializeField, Required] private GameObject timeBombPrefab;
    [SerializeField] private float distance;
    [SerializeField] private float timeCombatAbilityCooldown;

    private GameObject timeBomb;

    private Transform mainCameraTransform;

    private float timeWaitTime = 0;

    private bool timeIsActive;

    protected override void Awake()
    {
        base.Awake();

        mainCameraTransform = Camera.main.transform;
    }

    #region Events

    private void OnEnable()
    {
        inputManager.OnPowerSelect += OnPowerSelected;
        inputManager.OnCombatAbility += CombatAbility;
        inputManager.OnPuzzleAbility += PuzzleAbility;
    }

    private void OnPowerSelected(Vector2 power)
    {
        if (power[0] == 1)
        {
            timeIsActive = true;
            gameManager.PlayerChangePower(PowerType.Time);
        }
        else
        {
            timeIsActive = false;
        }
    }

    private void CombatAbility()
    {
        if (timeIsActive && timeWaitTime <= 0)
        {
            timeWaitTime = timeCombatAbilityCooldown;
            player.ID.playerEvents.OnGiveAimPosition += UseCombatAbility;
            player.ID.playerEvents.OnGetAimPosition?.Invoke();
        }
    }

    private void UseCombatAbility(Vector3 aimPosition)
    {
        if (timeIsActive)
        {
            if (aimPosition != Vector3.zero && aimPosition != Vector3.positiveInfinity && aimPosition != Vector3.negativeInfinity)
            {
                UseTimeBomb(aimPosition);
            }
            else
            {
                Vector3 forward = mainCameraTransform.forward;
                forward.y = 0f;
                forward.Normalize();

                UseTimeBomb(transform.position + forward * distance);
            }
        }
    }

    private void PuzzleAbility()
    {

    }

    private void OnDisable()
    {
        if (timeIsActive)
        {
            timeIsActive = false;
            gameManager.PlayerChangePower(PowerType.None);
        }

        inputManager.OnPowerSelect -= OnPowerSelected;
        inputManager.OnCombatAbility -= CombatAbility;
        inputManager.OnPuzzleAbility -= PuzzleAbility;
    }

    #endregion

    private void Update()
    {
        timeWaitTime -= Time.deltaTime;
    }

    #region Methods

    private void UseTimeBomb(Vector3 spawnPosition)
    {
        if (!timeBomb)
        {
            timeBomb = Instantiate(timeBombPrefab, spawnPosition, Quaternion.identity);
        }
        else if (timeBomb && !timeBomb.activeSelf)
        {
            timeBomb.transform.position = spawnPosition;
            timeBomb.SetActive(true);
        }
        else
        {
            timeBomb.SetActive(false);
        }
    }

    #endregion
}
