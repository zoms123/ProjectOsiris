using System;
using UnityEngine;
using static UnityEngine.UI.Image;

public class InputGravityPowerSystem : PlayerSystem
{
    [Header("References")]
    [SerializeField, Required] private InputManagerSO inputManager;
    [SerializeField, Required] private GameManagerSO gameManager;

    [Header("Overlap Settings")]
    [SerializeField, Required] private Transform overlapSphereStartPointTransform;
    [SerializeField] private Vector3 offsetDirection = Vector3.up;
    [SerializeField] private float offsetValue = 1;
    [SerializeField] private float overlapSphereRadius;
    [SerializeField] private float minDistance;
    [SerializeField] private LayerMask interactableLayerMask;

    [Header("Gravity Power Settings")]
    [SerializeField, Required] GameObject zeroGravityZonePrefab;
    [SerializeField, Required] Transform attachPointTransform;
    [SerializeField] private float controlMovementSpeed = 5f;
    [SerializeField] private float gravityCombatAbilityCooldown;
    [SerializeField] private float cameraTopClampUsingGravity = 20.0f;
    [SerializeField] private float cameraBottomClampUsingGravity = -70.0f;

    private GameObject zeroGravityZone;

    private IInteractable interactable;
    private IAttachable attachable;
    private IMovable movable;
    private ILosableObject losableObjet;

    private Rigidbody targetRigidbody;

    private Transform mainCameraTransform;

    private Vector3 overlapSphereEndPoint;

    private float gravityWaitTime = 0;
    private float inputDirectionY;
    private float inputDirectionZ;

    private bool gravityIsActive;
    private bool liftingObjectWithGravity;

    protected override void Awake()
    {
        base.Awake();

        mainCameraTransform = Camera.main.transform;
    }

    #region Events

    private void OnEnable()
    {
        inputManager.OnPowerSelect += OnPowerSelected;
        inputManager.OnCombatAbility += UseCombatAbility;
        inputManager.OnPuzzleAbility += PuzzleAbility;
    }

    private void OnPowerSelected(Vector2 power)
    {
        if (power[1] == -1)
        {
            gravityIsActive = true;
            gameManager.PlayerChangePower(PowerType.Gravity);
        }
        else
        {
            gravityIsActive = false;
        }
    }

    private void UseCombatAbility()
    {
        if (gravityIsActive && gravityWaitTime <= 0)
        {
            gravityWaitTime = gravityCombatAbilityCooldown;
            player.ID.playerEvents.OnUseCombatAbility.Invoke(); // play combat ability animation
            player.ID.playerEvents.OnFirePower += FireGravity;
        }
    }
    private void FireGravity(Vector3 aimPosition)
    {
        if (gravityIsActive)
        {
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
        player.ID.playerEvents.OnFirePower -= FireGravity;
    }


    private void PuzzleAbility()
    {
        if (gravityIsActive)
        {
            if(interactable == null)
            {
                player.ID.playerEvents.OnAimPositionReceived += UsePuzzleAbility;
                player.ID.playerEvents.OnGetAimPosition?.Invoke();
            }
            else
            {
                OnInteractableLost();
            }
        }
    }

    private void UsePuzzleAbility(Vector3 aimPosition, bool isAiming)
    {

        if (gravityIsActive)
        {
            if (interactable == null)
            {
                Vector3 direction = (aimPosition - overlapSphereStartPointTransform.position).normalized;
                
                if(Physics.Raycast(overlapSphereStartPointTransform.position, direction, out RaycastHit hitInfo, minDistance, interactableLayerMask))
                {
                    interactable = hitInfo.collider.GetComponent<IInteractable>();
                    attachable = hitInfo.collider.GetComponent<IAttachable>();
                    movable = hitInfo.collider.GetComponent<IMovable>();
                    losableObjet = hitInfo.collider.GetComponent<ILosableObject>();
                    if (interactable != null && interactable.CanInteract(PowerType.Gravity))
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
                            player.ID.playerEvents.OnLockRotation?.Invoke();
                            player.ID.playerEvents.OnChangeCameraRange.Invoke(cameraTopClampUsingGravity, cameraBottomClampUsingGravity);
                            inputManager.PuzzleGravityAbilityEnabled();
                            inputManager.OnControlObjectXY += ControlObjectXY;
                            inputManager.OnControlObjectZ += ControlObjectZ;
                            targetRigidbody = movable.GetRigidbody();
                            liftingObjectWithGravity = true;
                        }
                    }
                }
                player.ID.playerEvents.OnAimPositionReceived -= UsePuzzleAbility;
            }
        }
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
            player.ID.playerEvents.OnUnlockRotation?.Invoke();
            player.ID.playerEvents.OnResetCameraRange?.Invoke();
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

    private void ControlObjectXY(Vector2 direction)
    {
        inputDirectionY = direction[1];
    }

    private void ControlObjectZ(Vector2 direction)
    {
        if (Math.Abs(direction[1]) > 0.3)
            inputDirectionZ = direction[1];
        else
            inputDirectionZ = 0f;
    }

    private void OnDisable()
    {
        if (gravityIsActive)
        {
            gravityIsActive = false;
            gameManager.PlayerChangePower(PowerType.None);
        }

        inputManager.PuzzleGravityAbilityDisabled();

        inputManager.OnPowerSelect -= OnPowerSelected;
        inputManager.OnCombatAbility -= UseCombatAbility;
        inputManager.OnPuzzleAbility -= PuzzleAbility;
    }

    #endregion

    private void Update()
    {
        overlapSphereEndPoint = overlapSphereStartPointTransform.position + offsetDirection * offsetValue;

        gravityWaitTime -= Time.deltaTime;

        if (liftingObjectWithGravity && attachable.Attached)
            LiftingObjectWithGravityMovement();
    }

    #region Methods

    private void LiftingObjectWithGravityMovement()
    {
        targetRigidbody.velocity = Vector3.zero;
        Vector3 targetPosition = movable.GetPosition();
        Vector3 targetLocalPosition = movable.GetLocalPosition();

        if (targetPosition.y < attachPointTransform.position.y)
            movable.SetPosition(new Vector3(targetPosition.x, attachPointTransform.position.y, targetPosition.z));

        else if (targetLocalPosition.z < -1f)
            movable.SetLocalPosition(new Vector3(targetLocalPosition.x, targetLocalPosition.y, -1));

        else if (targetPosition.y > attachPointTransform.position.y + 4.5f)
            movable.SetPosition(new Vector3(targetPosition.x, attachPointTransform.position.y + 4.5f, targetPosition.z));

        else if (targetLocalPosition.z > 4.5f)
            movable.SetLocalPosition(new Vector3(targetLocalPosition.x, targetLocalPosition.y, 4.5f));

        else if (targetLocalPosition.x >= 0.5)
            targetRigidbody.AddForce(GetMoveDirection2(-1) * 2, ForceMode.Impulse);

        else if (targetLocalPosition.x <= -0.5)
            targetRigidbody.AddForce(GetMoveDirection2(1) * 2, ForceMode.Impulse);

        else
            targetRigidbody.AddForce(GetMoveDirection() * controlMovementSpeed, ForceMode.Impulse);
    }

    private Vector3 GetMoveDirection()
    {
        Vector3 forward = mainCameraTransform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 moveDir = forward * inputDirectionY;
        moveDir.y = inputDirectionZ > 0 ? 1 : (inputDirectionZ < 0 ? -1 : 0);
        moveDir.Normalize();

        return moveDir;
    }

    private Vector3 GetMoveDirection2(float direction)
    {
        Vector3 right = mainCameraTransform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 moveDir = right * direction;
        moveDir.y = 0f;
        moveDir.Normalize();

        return moveDir;
    }

    private void ChangeAttachableParent(Transform parentTransform)
    {
        if (attachable != null)
        {
            attachable.ChangeParent(parentTransform);
        }
    }

    #endregion

    #region Debug

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(overlapSphereStartPointTransform.position, overlapSphereRadius);
    }

    #endregion
}
