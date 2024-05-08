using System.Linq;
using UnityEngine;

public class PlayerPowersController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManagerSO inputManager;

    [Header("Overlap")]
    [SerializeField] private Transform overlapSphereStartPoint;
    [SerializeField] private Vector3 offsetDirection = Vector3.up;
    [SerializeField] private float offsetValue = 1;
    [SerializeField] private float overlapSphereRadius;

    [Header("Gravity Power")]
    [SerializeField] GameObject zeroGravityZonePrefab;
    [SerializeField] float zeroGravityZoneOffset;
    [SerializeField] Transform attachPoint;

    [Header("Crystal Power")]
    [SerializeField] private GameObject throwableCrystalPrefab;
    [SerializeField] private Transform firePoint;

    private GameObject zeroGravityZone;
    private PlayerManager playerManager;

    private IAttachable attachable;
    private IInteractable interactable;
    private Collider interactableCollider;
    private Vector3 overlapSphereEndPoint;

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

    private void Update()
    {
        overlapSphereEndPoint = overlapSphereStartPoint.position + offsetDirection * offsetValue;
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
        Transform targetTransform = GetComponent<TargetLockOn>().CurrentTarget;
        if(targetTransform != null)
        {
            if (!zeroGravityZone)
            {
                zeroGravityZone = Instantiate(zeroGravityZonePrefab, targetTransform.position, Quaternion.identity);
            }
            else if (zeroGravityZone && !zeroGravityZone.activeSelf)
            {
                zeroGravityZone.transform.position = targetTransform.position;
                zeroGravityZone.SetActive(true);
            }
            else
            {
                zeroGravityZone.SetActive(false);
            }
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
            Collider[] collidersTouched = Physics.OverlapCapsule(overlapSphereStartPoint.position, overlapSphereEndPoint, overlapSphereRadius);
            foreach (Collider collider in collidersTouched)
            {
                interactable = collider.GetComponent<IInteractable>();
                attachable = collider.GetComponent<IAttachable>();
                if (interactable != null && interactable.CanInteract(playerManager.CurrentPowerType))
                {
                    ChangeAttachableParent(attachPoint);
                    interactable.OnLoseObject += OnInteractableLost;
                    interactable.Interact();
                    interactableCollider = collider;
                    if(attachable == null)
                    {
                        interactable = null;
                        interactableCollider = null;
                    }
                    break;
                }
            }
        } 
        else if (interactable.CanInteract(playerManager.CurrentPowerType))
        {
            Collider[] collidersTouched = Physics.OverlapCapsule(overlapSphereStartPoint.position, overlapSphereEndPoint, overlapSphereRadius);
            if (collidersTouched.Contains(interactableCollider))
            {
                interactable.OnLoseObject -= OnInteractableLost;
                ChangeAttachableParent(null);
                interactable.Interact();
                interactable = null;
            }
        }
        
    }

    private void OnInteractableLost()
    {
        interactable.OnLoseObject -= OnInteractableLost;
        ChangeAttachableParent(null);
        interactable.Interact();
        interactable = null;
        attachable = null;
    }

    private void ChangeAttachableParent(Transform parentTransform)
    {
        if (attachable != null)
        {
            attachable.ChangeParent(parentTransform);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(overlapSphereStartPoint.position, overlapSphereRadius);
    }
}
