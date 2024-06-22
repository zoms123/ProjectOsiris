using System;
using System.Linq;
using UnityEngine;

public class PlayerPowersController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField, Required] private InputManagerSO inputManager;

    [Header("Overlap")]
    [SerializeField, Required] private Transform overlapSphereStartPointTransform;
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
    [SerializeField, Required] GameObject zeroGravityZonePrefab;
    [SerializeField] float zeroGravityZoneOffset;
    [SerializeField, Required] Transform attachPointTransform;
    [SerializeField] private float controlMovementSpeed;

    private bool liftingObjectWithGravity;
    private float inputDirectionY;
    private float inputDirectionZ;
    private Rigidbody targetRigidbody;

    [Header("Crystal Power")]
    [SerializeField, Required] private GameObject throwableCrystalPrefab;
    [SerializeField, Required] private Transform firePointTransform;

    [Header("Time Power")]
    [SerializeField, Required] private GameObject timeBombPrefab;
    [SerializeField] private float distance;

    private GameObject timeBomb;
    private Transform mainCameraTransform;

    private GameObject zeroGravityZone;
    private PlayerManager playerManager;

    private IAttachable attachable;
    private IInteractable interactable;
    private IMovable movable;
    private ILosableObject losableObjet;
    private Vector3 overlapSphereEndPoint;

    protected void Awake()
    {
        mainCameraTransform = Camera.main.transform;
        playerManager = GetComponent<PlayerManager>();
    }

    private void Start()
    {
        ObjectPooler.Instance.CreatePool(throwableCrystalPrefab, 10);
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
        overlapSphereEndPoint = overlapSphereStartPointTransform.position + offsetDirection * offsetValue;

        gravityWaitTime -= Time.deltaTime;
        crystalWaitTime -= Time.deltaTime;
        timeWaitTime -= Time.deltaTime;
        shadowWaitTime -= Time.deltaTime;

        if (liftingObjectWithGravity && attachable.Attached)
            LiftingObjectWithGravityMovement();
    }

    private void LiftingObjectWithGravityMovement()
    {
        targetRigidbody.velocity = Vector3.zero;
        Vector3 targetPosition = movable.GetPosition();
        Vector3 targetLocalPosition = movable.GetLocalPosition();

        if (targetPosition.y < attachPoint.transform.position.y)
            movable.SetPosition(new Vector3(targetPosition.x, attachPoint.transform.position.y, targetPosition.z));

        else if (targetLocalPosition.z < -1f)
            movable.SetLocalPosition(new Vector3(targetLocalPosition.x, targetLocalPosition.y, -1));

        else if (targetPosition.y > attachPoint.transform.position.y + 4.5f)
            movable.SetPosition(new Vector3(targetPosition.x, attachPoint.transform.position.y + 4.5f, targetPosition.z));

        else if (targetLocalPosition.z > 4.5f)
            movable.SetLocalPosition(new Vector3(targetLocalPosition.x, targetLocalPosition.y, 4.5f));
            
        else
            targetRigidbody.AddForce(GetMoveDirection() * controlMovementSpeed, ForceMode.Impulse);
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
        Vector3 aimPosition = playerManager.Aiming.GetAimPosition();
        if (aimPosition != Vector3.zero && aimPosition != Vector3.positiveInfinity && aimPosition != Vector3.negativeInfinity)
        {
            if (!zeroGravityZone)
            {
                zeroGravityZone = Instantiate(zeroGravityZonePrefab, aimPosition, Quaternion.identity);
            }
            else if (zeroGravityZone && !zeroGravityZone.activeSelf)
            {
                zeroGravityZone.transform.position = aimPosition;
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
        GameObject crystalObject = ObjectPooler.Instance.Spawn(throwableCrystalPrefab, firePointTransform.transform.position, Quaternion.identity);
        if (crystalObject != null && crystalObject.TryGetComponent<ThrowableCrystal>(out var crystal))
        {
            Vector3 aimPosition = playerManager.Aiming.GetAimPosition();
            if (aimPosition != Vector3.zero && aimPosition != Vector3.positiveInfinity && aimPosition != Vector3.negativeInfinity)
            {
                Vector3 targetDirection = (aimPosition - firePointTransform.position).normalized;
                Debug.DrawRay(firePointTransform.position, targetDirection, Color.blue);
                crystal.Initialize(targetDirection, tag);
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

    private void TimeCombatAbility()
    {
        Vector3 aimPosition = playerManager.Aiming.GetAimPosition();
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
        if (interactable == null)
        {
            Collider[] collidersTouched = Physics.OverlapCapsule(overlapSphereStartPointTransform.position, overlapSphereEndPoint, overlapSphereRadius);
            foreach (Collider collider in collidersTouched)
            {
                interactable = collider.GetComponent<IInteractable>();
                attachable = collider.GetComponent<IAttachable>();
                movable = collider.GetComponent<IMovable>();
                losableObjet = collider.GetComponent<ILosableObject>();
                if (interactable != null && interactable.CanInteract(playerManager.CurrentPowerType))
                {
                    ChangeAttachableParent(attachPointTransform);
                    if (losableObjet != null)
                        losableObjet.OnLoseObject += OnInteractableLost;
                    interactable.Interact();
                    if (attachable == null)
                    {
                        interactable = null;
                    }
                    else if (movable != null)
                    {
                        playerManager.Locomotion.lockRotation = true;
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
        if (Math.Abs(direction[1]) > 0.7)
            inputDirectionZ = direction[1];
        else
            inputDirectionZ = 0f;
    }

    private void OnInteractableLost()
    {
        if (losableObjet != null)
            losableObjet.OnLoseObject -= OnInteractableLost;
        ChangeAttachableParent(null);
        interactable.Interact();
        interactable = null;
        attachable = null;

        if (movable != null)
        {
            playerManager.Locomotion.lockRotation = false;
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
        Gizmos.DrawWireSphere(overlapSphereStartPointTransform.position, overlapSphereRadius);
    }
    #endregion
}