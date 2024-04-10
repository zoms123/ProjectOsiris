using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CrystalController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManagerSO inputManager;
    [SerializeField] private GameManagerSO gameManager;
    [SerializeField] private GameObject throwableCrystalPrefab;
    [SerializeField] private Transform firePoint;

    private IInteractable interactable;

    private void OnEnable()
    {
        inputManager.OnCombatAbility += Fire;
        inputManager.OnPuzzleAbility += Interact;
    }

    private void OnDisable()
    {
        inputManager.OnCombatAbility -= Fire;
        inputManager.OnPuzzleAbility -= Interact;
    }

    private void Fire()
    {
        if(gameManager.CurrentPowerType == PowerType.Crystal)
        {
            Transform currentTarget = GetComponent<TargetLockOn>().CurrentTarget;
            GameObject throwableCrystal = Instantiate(throwableCrystalPrefab, firePoint.position, Quaternion.identity);
            Vector3 direction = transform.forward;
            if (currentTarget != null)
            {
                direction = currentTarget.position - transform.position;
            }
            throwableCrystal.GetComponent<ThrowableCrystal>().Move(direction);
        }
    }

    private void Interact()
    {
        if (gameManager.CurrentPowerType == PowerType.Crystal && interactable != null && interactable.CanInteract())
        {
            interactable.Interact(PowerType.Crystal);
        }
    }

    #region Collisions and Triggers

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interact = other.GetComponent<IInteractable>();
        if (interact != null)
        {
                interactable = interact;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interact = other.GetComponent<IInteractable>();
        if (interact != null)
        {
            interactable = null;
        }
    }
    #endregion
}
