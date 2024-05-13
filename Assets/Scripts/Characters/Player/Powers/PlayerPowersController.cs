using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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
    [SerializeField] private float controlMovementSpeed;
    
    private bool liftingObjectWithGravity;
    private float inputDirectionY;
    private float inputDirectionZ;
    private Rigidbody targetRigidbody;

    [Header("Crystal Power")]
    [SerializeField] private GameObject throwableCrystalPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Time Power")]
    [SerializeField] private GameObject timeBombPrefab;
    [SerializeField] private float distance;

    private GameObject timeBomb;
    private Transform mainCameraTransform;

    private GameObject zeroGravityZone;
    private PlayerManager playerManager;
    private PlayerLocomotion playerlocomotion;
    private TargetLockOn targetLockOn;

    private IAttachable attachable;
    private IInteractable interactable;
    private IMovable movable;
    private Vector3 overlapSphereEndPoint;

    protected void Awake()
    {
        mainCameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        targetLockOn = GetComponent<TargetLockOn>();
        playerlocomotion = playerManager.Locomotion;
        mainCameraTransform = Camera.main.transform;

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

        if (liftingObjectWithGravity)
            LiftingObjectWithGravityMovement();
    }

    private void LiftingObjectWithGravityMovement()
    {
        targetRigidbody.velocity = Vector3.zero;
        Vector3 targetPosition = movable.GetPosition();
        Vector3 targetLocalPosition = movable.GetLocalPosition();

        if (Vector3.Distance(targetPosition, attachPoint.transform.position) < 4.5f
            && targetPosition.y >= attachPoint.transform.position.y
            && targetLocalPosition.z >= -1f)
            targetRigidbody.AddForce(GetMoveDirection() * controlMovementSpeed, ForceMode.Impulse);

        else if (targetPosition.y < attachPoint.transform.position.y)
            targetRigidbody.AddForce(Vector3.up * 2f, ForceMode.Impulse);

        else
        {
            Vector3 direction = (attachPoint.transform.position - targetPosition).normalized;
            targetRigidbody.AddForce(direction * 2f, ForceMode.Impulse);
        }

    }

    public Vector3 GetMoveDirection()
    {
        Vector3 forward = mainCameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 moveDir = forward * inputDirectionY;
        moveDir.y = inputDirectionZ;
        moveDir.Normalize();

        return moveDir;
    }

    #region Combat Abilitys

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
                    CrystalCombatAbility();
                    crystalWaitTime = crystalCombatAbilityCooldown;
                }
                break;

            case PowerType.Time:
                if (timeWaitTime <= 0)
                {
                    TimeCombatAbility();
                    timeWaitTime = timeCombatAbilityCooldown;
                }
                break;

            case PowerType.Shadow:
                if (shadowWaitTime <= 0)
                {
                    ShadowCombatAbility();
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
        Transform currentTarget = GetComponent<TargetLockOn>().CurrentTarget;

        if (currentTarget != null)
            UseTimeBomb(currentTarget.position);
        else
        {
            Vector3 forward = mainCameraTransform.forward;
            forward.y = 0f;
            forward.Normalize();

            UseTimeBomb(transform.position + forward * distance);
        }
    }

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

    private void ShadowCombatAbility()
    {

    }

    #endregion

    private void PuzzleAbility()
    {
        if(interactable == null)
        {
            Collider[] collidersTouched = Physics.OverlapCapsule(overlapSphereStartPoint.position, overlapSphereEndPoint, overlapSphereRadius);
            foreach (Collider collider in collidersTouched)
            {
                interactable = collider.GetComponent<IInteractable>();
                attachable = collider.GetComponent<IAttachable>();
                movable = collider.GetComponent<IMovable>();
                if (interactable != null && interactable.CanInteract(playerManager.CurrentPowerType))
                {
                    ChangeAttachableParent(attachPoint);
                    interactable.OnLoseObject += OnInteractableLost;
                    interactable.Interact();
                    if(attachable == null)
                    {
                        interactable = null;
                    }
                    else if (movable != null)
                    {
                        playerlocomotion.lockRotation = true;
                        inputManager.PuzzleGravityAbilityEnabled();
                        inputManager.OnControlObjectXY += ControlObjectXY;
                        inputManager.OnControlObjectZ += ControlObjectZ;
                        targetRigidbody = movable.GetRigidbody();
                        liftingObjectWithGravity = true;
                    }
                        
                    break;
                }
            }
        } 
        else if (interactable.CanInteract(playerManager.CurrentPowerType))
        {
            OnInteractableLost();
        }
        
    }

    private void ControlObjectXY(Vector2 direction)
    {
        inputDirectionY = direction[1];
    }

    private void ControlObjectZ(Vector2 direction)
    {
        inputDirectionZ = direction[1];
    }

    private void OnInteractableLost()
    {
        interactable.OnLoseObject -= OnInteractableLost;
        ChangeAttachableParent(null);
        interactable.Interact();
        interactable = null;
        attachable = null;

        if (movable != null)
        {
            playerlocomotion.lockRotation = false;
            inputManager.PuzzleGravityAbilityDisabled();
            inputManager.OnControlObjectXY -= ControlObjectXY;
            inputManager.OnControlObjectZ -= ControlObjectZ;
            liftingObjectWithGravity = false;
            targetRigidbody.velocity = Vector3.zero;
            targetRigidbody = null;
            inputDirectionY = 0f;
            inputDirectionZ = 0f;
            movable = null;
        }
    }

    private void ChangeAttachableParent(Transform parentTransform)
    {
        if (attachable != null)
        {
            attachable.ChangeParent(parentTransform);
        }
    }

    #region Debug

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(overlapSphereStartPoint.position, overlapSphereRadius);
    }
    #endregion
}