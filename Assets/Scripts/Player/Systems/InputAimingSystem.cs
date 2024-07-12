using Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class InputAimingSystem : PlayerSystem
{
    [Header("References")]
    [SerializeField, Required] private InputManagerSO inputManager;
    [SerializeField, Required] private GameManagerSO gameManager;

    [Header("Aim Settings")]
    [SerializeField, Required] private Transform aimLookAtTransform;
    [SerializeField] private LayerMask aimingMask;
    [SerializeField] private float aimCameraSensitivity = 0.5f;
    [SerializeField] private float aimFOV = 50.0f;
    [SerializeField] private float aimTargetSmoothSpeed = 20;
    [SerializeField] private float playerRotationSpeed = 20;
    [SerializeField] private float aimTransitionSpeed = 10f;

    private float aimLayerWeight = 0f;
    private float aimLayerTargetWeight = 0f;

    private bool isAiming = false;

    private void Start()
    {
        LoadPlayerPrefs();
    }

    #region Events

    private void OnEnable()
    {
        inputManager.OnAim += HandleAim;

        player.ID.playerEvents.OnGetAimPosition += GiveAimPosition;

        gameManager.OnUpdateAimSensitivity += UpdateAimSensitivity;
    }


    private void HandleAim(bool isAiming)
    {
        this.isAiming = isAiming;
        aimLayerTargetWeight = isAiming ? 1f : 0f;

        player.ID.playerEvents.OnUpdateCameraSettings?.Invoke(isAiming, aimCameraSensitivity, aimFOV);
        player.ID.playerEvents.OnPlayerAim?.Invoke(isAiming);
    }

    private void GiveAimPosition()
    {
        if (isAiming)
        {
            player.ID.playerEvents.OnGiveAimPosition?.Invoke(GetAimPosition());
        }
    }

    private void UpdateAimSensitivity()
    {
        if (PlayerPrefs.HasKey("masterAimSensitivity"))
        {
            aimCameraSensitivity = PlayerPrefs.GetFloat("masterAimSensitivity");
            player.ID.playerEvents.OnUpdateCameraSettings?.Invoke(isAiming, aimCameraSensitivity, aimFOV);
        }
    }

    private void OnDisable()
    {
        inputManager.OnAim -= HandleAim;

        player.ID.playerEvents.OnGetAimPosition -= GiveAimPosition;

        gameManager.OnUpdateAimSensitivity -= UpdateAimSensitivity;
    }

    #endregion

    private void Update()
    {
        // apply upper body layer and body rig weights
        aimLayerWeight = Mathf.Lerp(aimLayerWeight, aimLayerTargetWeight, Time.deltaTime * aimTransitionSpeed);

        player.ID.playerEvents.OnUpdateAimParameters?.Invoke(1, aimLayerWeight, aimLayerTargetWeight);

        HandleAimPosition();

        if (isAiming) RotatePlayerTowardsAimDirection();
    }

    #region Methods

    private void LoadPlayerPrefs()
    {
        UpdateAimSensitivity();
    }

    private void HandleAimPosition()
    {
        Vector3 targetPosition = GetAimPosition();

        // make sure the position obtained is valid before assigning it
        if (targetPosition != Vector3.zero && targetPosition != Vector3.positiveInfinity && targetPosition != Vector3.negativeInfinity)
        {
            aimLookAtTransform.position = Vector3.Lerp(aimLookAtTransform.position, targetPosition, Time.deltaTime * aimTargetSmoothSpeed);
        }
    }

    public Vector3 GetAimPosition()
    {
        Vector2 screenCenterPoint = new(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, aimingMask))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
            return hit.point;
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * float.MaxValue, Color.red);
            return ray.origin + ray.direction * 1000f;
        }
    }

    private void RotatePlayerTowardsAimDirection()
    {
        Vector3 worldAimTarget = aimLookAtTransform.position;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * playerRotationSpeed);
    }

    #endregion
}
