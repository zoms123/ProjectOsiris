using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPowersController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManagerSO inputManager;

    [Header("Overlap")]
    [SerializeField] private Transform overlapSphereTransform;
    [SerializeField] private float overlapSphereRadius;

    [Header("Gravity Power")]
    [SerializeField] GameObject zeroGravityZonePrefab;
    [SerializeField] float zeroGravityZoneOffset;

    [Header("Crystal Power")]
    [SerializeField] private GameObject throwableCrystalPrefab;
    [SerializeField] private Transform firePoint;

    private GameObject zeroGravityZone;
    private PlayerManager playerManager;

    private IInteractable interactable;

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    private void OnEnable()
    {
        inputManager.OnCombatAbility += CombatAbility;
        inputManager.OnPuzzleAbility += PuzzleAbility;
    }

    private void OnDisable()
    {
        inputManager.OnCombatAbility -= CombatAbility;
        inputManager.OnPuzzleAbility -= PuzzleAbility;
    }

    private void CombatAbility()
    {
        switch (playerManager.CurrentPowerType)
        {
            case PowerType.Gravity:
                GravityCombatAbility();
                break;

            case PowerType.Crystal:
                CrystalCombatAbility();
                break;

            case PowerType.Time:
                TimeCombatAbility();
                break;

            case PowerType.Shadow:
                ShadowCombatAbility();
                break;

            default:
                break;
        }
    }

    private void GravityCombatAbility()
    {
        if (!zeroGravityZone)
        {
            Vector3 position = transform.position + Vector3.forward * zeroGravityZoneOffset;
            zeroGravityZone = Instantiate(zeroGravityZonePrefab, position, Quaternion.identity);
        }
        else if (zeroGravityZone && !zeroGravityZone.activeSelf)
        {
            Vector3 position = transform.position + Vector3.forward * zeroGravityZoneOffset;
            zeroGravityZone.transform.position = position;
            zeroGravityZone.SetActive(true);
        }
        else
        {
            zeroGravityZone.SetActive(false);
        }
    }

    private void CrystalCombatAbility()
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

    private void TimeCombatAbility()
    {

    }

    private void ShadowCombatAbility()
    {

    }

    private void PuzzleAbility()
    {
        if(interactable == null)
        {
            Collider[] collidersTouched = Physics.OverlapSphere(overlapSphereTransform.position, overlapSphereRadius);
            foreach (Collider collider in collidersTouched)
            {
                interactable = collider.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanInteract(playerManager.CurrentPowerType))
                {
                    interactable.Interact();
                    break;
                }
            }
        } 
        else if (interactable.CanInteract(playerManager.CurrentPowerType))
        {
            interactable.Interact();
            interactable = null;
        }
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(overlapSphereTransform.position, overlapSphereRadius);
    }
}
