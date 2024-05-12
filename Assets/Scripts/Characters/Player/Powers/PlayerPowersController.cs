using System.Linq;
using System.Threading;
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

    [Header("Cooldown")]
    [SerializeField] private float gravityCombatAbilityCooldown;
    [SerializeField] private float crystalCombatAbilityCooldown;
    [SerializeField] private float timeCombatAbilityCooldown;
    [SerializeField] private float shadowCombatAbilityCooldown;

    private float gravityWaitTime = 0;
    private float crystalWaitTime = 0;
    private float timeWaitTime = 0;
    private float shadowWaitTime = 0;

    [Header("Gravity Power")]
    [SerializeField] GameObject zeroGravityZonePrefab;
    [SerializeField] float zeroGravityZoneOffset;
    [SerializeField] Transform attachPoint;
    private TargetLockOn targetLockOn;

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
        targetLockOn = GetComponent<TargetLockOn>();

        ObjectPool.Initialize(throwableCrystalPrefab);
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

        gravityWaitTime -= Time.deltaTime;
        crystalWaitTime -= Time.deltaTime;
        timeWaitTime -= Time.deltaTime;
        shadowWaitTime -= Time.deltaTime;
    }

    private void CombatAbility()
    {
        switch (playerManager.CurrentPowerType)
        {
            case PowerType.Gravity:
                if (gravityWaitTime <= 0)
                {
                    GravityCombatAbility();
                    gravityWaitTime = gravityCombatAbilityCooldown;
                }
                    
                break;

            case PowerType.Crystal:
                if (crystalWaitTime <= 0)
                {
                    GravityCombatAbility();
                    crystalWaitTime = crystalCombatAbilityCooldown;
                }
                break;

            case PowerType.Time:
                if (timeWaitTime <= 0)
                {
                    GravityCombatAbility();
                    timeWaitTime = timeCombatAbilityCooldown;
                }
                break;

            case PowerType.Shadow:
                if (shadowWaitTime <= 0)
                {
                    GravityCombatAbility();
                    shadowWaitTime = shadowCombatAbilityCooldown;
                }
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
        GameObject crystalObject = ObjectPool.GetObject(throwableCrystalPrefab);
        if (crystalObject != null)
        {
            crystalObject.transform.position = firePoint.position;
            crystalObject.transform.rotation = firePoint.rotation;
            ThrowableCrystal crystal = crystalObject.GetComponent<ThrowableCrystal>();
            if (crystal != null)
            {
                Vector3 targetDirection = firePoint.forward;
                if (targetLockOn.CurrentTarget)
                {
                    targetDirection = targetLockOn.CurrentTarget.position - firePoint.position;
                    targetDirection.Normalize();
                    //Debug.DrawRay(firePoint.position, targetDirection * 20, Color.cyan, 5f);
                }
                crystal.Initialize(targetDirection, tag);
            }
        }
    }

    //private void CrystalCombatAbility()
    //{
    //    Transform currentTarget = GetComponent<TargetLockOn>().CurrentTarget;
    //    GameObject throwableCrystal = Instantiate(throwableCrystalPrefab, firePoint.position, Quaternion.identity);
    //    Vector3 direction = transform.forward;
    //    if (currentTarget != null)
    //    {
    //        direction = currentTarget.position - transform.position;
    //    }
    //    throwableCrystal.GetComponent<ThrowableCrystal>().Move(direction);
    //}

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
