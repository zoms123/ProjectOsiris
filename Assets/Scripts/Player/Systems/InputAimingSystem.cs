using Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class InputAimingSystem : PlayerSystem
{
    [Header("References")]
    [SerializeField, Required] private InputManagerSO inputManager;
    [SerializeField, Required] private GameManagerSO gameManager;

    [Header("Camera Settings")]
    [SerializeField, Required] private CinemachineVirtualCamera playerCamera;
    [SerializeField, Required] private Transform cameraFollowTransform;
    [SerializeField] private float defaultCameraSensitivity = 1.0f;
    [SerializeField] private float aimCameraSensitivity = 0.5f;
    [SerializeField] private float aimFOV = 50.0f;
    [Tooltip("How far in degrees you can move the camera up")]
    [SerializeField] private float cameraTopClamp = 70.0f;
    [Tooltip("How far in degrees you can move the camera down")]
    [SerializeField] private float cameraBottomClamp = -30.0f;

    [Header("Animation Settings")]
    [SerializeField, Required] private Rig bodyRig;
    [SerializeField] private float aimTransitionSpeed = 5f;

    [Header("Aim Settings")]
    [SerializeField, Required] private Transform aimLookAtTransform;
    [SerializeField] private LayerMask aimingMask;
    [SerializeField] private float aimTargetSmoothSpeed = 20;
    [SerializeField] private float playerRotationSpeed = 20;

    private Vector2 lookInput;

    private float cameraSensitivity;
    private float defaultFOV;
    private float currentCameraFOV;
    private float aimLayerWeight = 0f;
    private float aimLayerTargetWeight = 0f;
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    private int invertY = 1;

    private bool isAiming = false;

    private void Start()
    {
        Cursor.visible = false;
        inputManager.SetCursorState(CursorLockMode.Locked);

        defaultFOV = playerCamera.m_Lens.FieldOfView;
        currentCameraFOV = defaultFOV;
        cameraSensitivity = defaultCameraSensitivity;

        bodyRig.weight = 0f;

        LoadGamePlayPrefs();
    }

    #region Events

    private void OnEnable()
    {
        inputManager.OnLook += HandleLook;
        inputManager.OnAim += HandleAim;

        player.ID.playerEvents.OnGetAimPosition += GiveAimPosition;

        gameManager.OnUpdateControllerSensitivity += UpdateControllerSensitivity;
        gameManager.OnUpdateAimSensitivity += UpdateAimSensitivity;
        gameManager.OnUpdateInvertY += UpdateInvertY;
    }

    private void HandleLook(Vector2 newLook)
    {
        lookInput = newLook;
    }

    private void HandleAim(bool isAiming)
    {
        this.isAiming = isAiming;
        currentCameraFOV = isAiming ? aimFOV : defaultFOV;
        aimLayerTargetWeight = isAiming ? 1f : 0f;
        cameraSensitivity = isAiming ? aimCameraSensitivity : defaultCameraSensitivity;

        player.ID.playerEvents.OnPlayerAim?.Invoke(isAiming);
    }

    private void GiveAimPosition()
    {
        if (isAiming)
        {
            player.ID.playerEvents.OnGiveAimPosition?.Invoke(GetAimPosition());
        }
    }

    private void UpdateControllerSensitivity()
    {
        if (PlayerPrefs.HasKey("masterSensitivity"))
            cameraSensitivity = PlayerPrefs.GetFloat("masterSensitivity");
    }

    private void UpdateAimSensitivity()
    {
        if (PlayerPrefs.HasKey("masterAimSensitivity"))
            aimCameraSensitivity = PlayerPrefs.GetFloat("masterAimSensitivity");
    }

    private void UpdateInvertY()
    {
        if (PlayerPrefs.HasKey("masterInvertY"))
            invertY = PlayerPrefs.GetInt("masterInvertY");
    }

    private void OnDisable()
    {
        inputManager.OnLook -= HandleLook;
        inputManager.OnAim -= HandleAim;

        player.ID.playerEvents.OnGetAimPosition -= GiveAimPosition;

        gameManager.OnUpdateControllerSensitivity -= UpdateControllerSensitivity;
        gameManager.OnUpdateAimSensitivity -= UpdateAimSensitivity;
        gameManager.OnUpdateInvertY -= UpdateInvertY;
    }

    #endregion

    private void Update()
    {
        // apply upper body layer and body rig weights
        aimLayerWeight = Mathf.Lerp(aimLayerWeight, aimLayerTargetWeight, Time.deltaTime * aimTransitionSpeed);
        bodyRig.weight = Mathf.Lerp(aimLayerWeight, aimLayerTargetWeight, Time.deltaTime * aimTransitionSpeed);
        player.ID.playerEvents.OnUpdateAimParameters?.Invoke(1, aimLayerWeight);

        // camera field of view transition
        playerCamera.m_Lens.FieldOfView = Mathf.Lerp(playerCamera.m_Lens.FieldOfView, currentCameraFOV, Time.deltaTime * aimTransitionSpeed);

        HandleAimPosition();

        if (isAiming) RotatePlayerTowardsAimDirection();
    }

    private void LateUpdate()
    {
        // if look input is moving
        if (lookInput.sqrMagnitude >= 0.01f)
        {
            cinemachineTargetYaw += lookInput.x * cameraSensitivity;
            cinemachineTargetPitch -= lookInput.y * cameraSensitivity * invertY;
        }

        // clamp rotations so values are limited 360 degrees
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, cameraBottomClamp, cameraTopClamp);

        // camera will follow this target
        cameraFollowTransform.rotation = Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0.0f);
    }

    #region Methods

    private void LoadGamePlayPrefs()
    {
        UpdateControllerSensitivity();
        UpdateAimSensitivity();
        UpdateInvertY();
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

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    #endregion
}
