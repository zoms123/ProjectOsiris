using UnityEngine;
using Cinemachine;

public class TargetLockOn : MonoBehaviour
{
    private Transform currentTarget;
    public Transform CurrentTarget { get { return currentTarget; } }
    private bool isLockActive = false;
    
    [Header("Settings")]

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float detectionRadius = 10f;
    [Tooltip("Smoothing factor for the rotation towards the current target")]
    [SerializeField] private float lookAtSmoothing = 2f;
    [Tooltip("Angle in degrees")]
    [SerializeField] private float maxDetectionAngle = 60f;
    [SerializeField] private float crosshairScale = 0.2f;
    [Tooltip("If true, applies a limit to the Y offset value for the target's look direction.This controls the vertical alignment when targeting an enemy.")]
    [SerializeField] private bool applyYOffsetLimit;

    private bool targetLocked;
    private float currentYOffset;
    private Vector3 targetLocator;

    [Header("References")]

    [Tooltip("Animator for switching cameras")]
    [SerializeField] private Animator cinemachineAnimator;
    [Tooltip("Canvas Transform for the crosshair of the locked target")]
    [SerializeField] private Transform lockOnCanvas;
    [SerializeField] private InputManagerSO inputManager;
    [SerializeField] private CinemachineVirtualCamera targetLockCamera;

    private Transform cam;
    private Animator anim;
    private CharacterMovement characterMovement;

    private void OnEnable()
    {
        inputManager.OnLockTarget += ToggleTargetLock;
    }

    private void Start()
    {
        cam = Camera.main.transform;
        anim = GetComponent<Animator>();
        characterMovement = GetComponent<CharacterMovement>();

        lockOnCanvas.gameObject.SetActive(false);
    }

    private void ToggleTargetLock()
    {
        isLockActive = !isLockActive;
        if (isLockActive)
        {
            currentTarget = ScanNearBy();
            if (currentTarget != null)
            {
                FoundTarget();
            }
        }
        else
        {
            ResetTarget();
        }
    }

    private void Update()
    {
        characterMovement.lockMovement = targetLocked;
        if (targetLocked)
        {
            if (!IsTargetInRange()) ResetTarget();
            LookAtTarget();
            UpdateTargetLocatorPosition();
        }
    }

    private void ResetTarget()
    {
        lockOnCanvas.gameObject.SetActive(false);
        currentTarget = null;
        targetLocked = false;
        anim.SetLayerWeight(1, 0);
        targetLockCamera.LookAt = null;
        cinemachineAnimator.Play("FreeLookCamera");
        Debug.Log("<color=orange>Target reset</color>");
    }

    private void FoundTarget()
    {
        lockOnCanvas.gameObject.SetActive(true);
        anim.SetLayerWeight(1, 1);
        targetLockCamera.LookAt = lockOnCanvas;
        cinemachineAnimator.Play("TargetCamera");
        targetLocked = true;
        Debug.Log("<color=green>" + currentTarget.gameObject.name + " locked.</color>");
    }

    private void LookAtTarget()
    {
        if (currentTarget == null)
        {
            ResetTarget();
            return;
        }

        lockOnCanvas.position = targetLocator;
        lockOnCanvas.localScale = Vector3.one * (cam.position - targetLocator).magnitude * crosshairScale;

        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * lookAtSmoothing);
    }

    private Transform ScanNearBy()
    {
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);
        float closestAngle = maxDetectionAngle;
        Transform closestTarget = null;

        if (nearbyTargets.Length <= 0) return null;

        foreach (Collider targetCollider in nearbyTargets)
        {
            if (targetCollider.CompareTag("Player"))
            {
                continue; // Skip player
            }

            Vector3 directionToTarget = targetCollider.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            if (angle < closestAngle)
            {
                closestAngle = angle;
                closestTarget = targetCollider.transform;
            }
        }
        
        if (!closestTarget) return null;

        // Set targetLocator position
        float h1 = closestTarget.GetComponent<CapsuleCollider>().height;
        float h2 = closestTarget.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2) / 2;
        currentYOffset = h - half_h;
        if (applyYOffsetLimit && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;

        targetLocator = closestTarget.position + new Vector3(0, currentYOffset, 0);

        if (Blocked())
        {
            Debug.DrawLine(transform.position, targetLocator, Color.red);
            return null;
        }
        Debug.DrawLine(transform.position, targetLocator, Color.green);

        return closestTarget;
    }

    private void UpdateTargetLocatorPosition()
    {
        if (currentTarget == null) return;

        float h1 = currentTarget.GetComponent<CapsuleCollider>().height;
        float h2 = currentTarget.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2) / 2;
        currentYOffset = h - half_h;
        if (applyYOffsetLimit && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;

        targetLocator = currentTarget.position + new Vector3(0, currentYOffset, 0);
    }

    private bool Blocked()
    {
        RaycastHit hit;
        if (Physics.Linecast(transform.position + Vector3.up * 0.5f, targetLocator, out hit))
        {
            if (!hit.transform.CompareTag("Enemy")) return true;
        }
        return false;
    }

    private bool IsTargetInRange()
    {
        float dis = (transform.position - targetLocator).magnitude;
        if (dis / 2 > detectionRadius) return false;
        else return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
