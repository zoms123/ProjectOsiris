using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCrystalPowerSystem : PlayerSystem
{
    [Header("References")]
    [SerializeField, Required] private InputManagerSO inputManager;
    [SerializeField, Required] private GameManagerSO gameManager;

    [Header("Overlap Settings")]
    [SerializeField, Required] private Transform overlapSphereStartPointTransform;
    [SerializeField] private Vector3 offsetDirection = Vector3.up;
    [SerializeField] private float offsetValue = 1;
    [SerializeField] private float overlapSphereRadius;

    [Header("Crystal Power Settings")]
    [SerializeField, Required] private GameObject throwableCrystalPrefab;
    [SerializeField, Required] private Transform firePointTransform;
    [SerializeField] private float crystalCombatAbilityCooldown;

    private IInteractable interactable;

    private Vector3 overlapSphereEndPoint;

    private float crystalWaitTime = 0;

    private bool crystalIsActive;

    private void Start()
    {
        ObjectPooler.Instance.CreatePool(throwableCrystalPrefab, 10);
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
        if (power[0] == -1)
        {
            crystalIsActive = true;
            gameManager.PlayerChangePower(PowerType.Crystal);
        }
        else
        {
            crystalIsActive = false;
        }
    }

    private void CombatAbility()
    {
        if (crystalIsActive && crystalWaitTime <= 0)
        {
            crystalWaitTime = crystalCombatAbilityCooldown;
            player.ID.playerEvents.OnGiveAimPosition += UseCombatAbility;
            player.ID.playerEvents.OnGetAimPosition?.Invoke();
        }
    }

    private void UseCombatAbility(Vector3 aimPosition)
    {
        if (crystalIsActive)
        {
            GameObject crystalObject = ObjectPooler.Instance.Spawn(throwableCrystalPrefab, firePointTransform.transform.position, Quaternion.identity);
            if (crystalObject != null && crystalObject.TryGetComponent<ThrowableCrystal>(out var crystal))
            {
                if (aimPosition != Vector3.zero && aimPosition != Vector3.positiveInfinity && aimPosition != Vector3.negativeInfinity)
                {
                    Vector3 targetDirection = (aimPosition - firePointTransform.position).normalized;

                    // set crystal rotation in the firing direction
                    Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
                    crystalObject.transform.rotation = lookRotation;

                    crystal.Initialize(targetDirection, gameObject);
                }
                else
                {
                    Debug.LogError("Invalid aim position");
                }
            }
            else
            {
                Debug.LogError("Failed to spawn or get ThrowableCrystal component.");
            }
        }
        player.ID.playerEvents.OnGiveAimPosition -= UseCombatAbility;
    }

    private void PuzzleAbility()
    {
        if (crystalIsActive && interactable == null)
        {
            Collider[] collidersTouched = Physics.OverlapCapsule(overlapSphereStartPointTransform.position, overlapSphereEndPoint, overlapSphereRadius);
            foreach (Collider collider in collidersTouched)
            {
                interactable = collider.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanInteract(PowerType.Crystal))
                {
                    interactable.Interact();
                    interactable = null;

                    break;
                }
            }
        }
    }

    private void OnDisable()
    {
        if (crystalIsActive)
        {
            crystalIsActive = false;
            gameManager.PlayerChangePower(PowerType.None);
        }

        inputManager.OnPowerSelect -= OnPowerSelected;
        inputManager.OnCombatAbility -= CombatAbility;
        inputManager.OnPuzzleAbility -= PuzzleAbility;
    }

    #endregion

    private void Update()
    {
        overlapSphereEndPoint = overlapSphereStartPointTransform.position + offsetDirection * offsetValue;

        crystalWaitTime -= Time.deltaTime;
    }
}
