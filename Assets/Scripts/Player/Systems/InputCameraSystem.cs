using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputCameraSystem : PlayerSystem
{
    [Header("References")]
    [SerializeField, Required] private InputManagerSO inputManager;
    [SerializeField, Required] private GameManagerSO gameManager;

    [Header("Camera Settings")]
    [SerializeField, Required] private CinemachineVirtualCamera playerCamera;
    [SerializeField, Required] private Transform cameraFollowTransform;
    [SerializeField] private float defaultCameraSensitivity = 1.0f;
    [SerializeField] private float transitionSpeedFOV = 10f;
    [Tooltip("How far in degrees you can move the camera up")]
    [SerializeField] private float defaultCameraTopClamp = 70.0f;
    [Tooltip("How far in degrees you can move the camera down")]
    [SerializeField] private float defaultCameraBottomClamp = -70.0f;

    private Vector2 lookInput;

    private float defaultFOV;
    private float currentCameraFOV;
    private float cameraSensitivity;
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    private float cameraTopClamp;
    private float cameraBottomClamp;

    private int invertY = 1;

    private void Start()
    {
        inputManager.SetCursorState(true);

        defaultFOV = playerCamera.m_Lens.FieldOfView;
        currentCameraFOV = defaultFOV;

        ResetCameraRange();

        cameraSensitivity = defaultCameraSensitivity;

        LoadPlayerPrefs();
    }

    #region Events

    private void OnEnable()
    {
        inputManager.OnLook += HandleLook;

        player.ID.playerEvents.OnUpdateCameraSettings += UpdateSettingsAfectedByAim;
        player.ID.playerEvents.OnChangeCameraRange += ChangeCameraRange;
        player.ID.playerEvents.OnResetCameraRange += ResetCameraRange;

        gameManager.OnUpdateControllerSensitivity += UpdateControllerSensitivity;
        gameManager.OnUpdateInvertY += UpdateInvertY;
    }

    private void HandleLook(Vector2 newLook)
    {
        lookInput = newLook;
    }

    private void UpdateSettingsAfectedByAim(bool isAiming, float aimCameraSensitivity, float aimFOV)
    {
        if (isAiming)
        {
            cameraSensitivity = aimCameraSensitivity;
            currentCameraFOV = aimFOV;
        }
        else
        {
            cameraSensitivity = defaultCameraSensitivity;
            currentCameraFOV = defaultFOV;
        }
    }

    private void ChangeCameraRange(float newCameraTopClamp, float newCameraBottomClamp)
    {
        cameraTopClamp = newCameraTopClamp;
        cameraBottomClamp = newCameraBottomClamp;
    }

    private void ResetCameraRange()
    {
        cameraTopClamp = defaultCameraTopClamp;
        cameraBottomClamp = defaultCameraBottomClamp;
    }

    private void UpdateControllerSensitivity()
    {
        if (PlayerPrefs.HasKey("masterSensitivity"))
        {
            defaultCameraSensitivity = PlayerPrefs.GetFloat("masterSensitivity");
            cameraSensitivity = defaultCameraSensitivity;
        }
    }

    private void UpdateInvertY()
    {
        if (PlayerPrefs.HasKey("masterInvertY"))
            invertY = PlayerPrefs.GetInt("masterInvertY");
    }

    private void OnDisable()
    {
        inputManager.OnLook -= HandleLook;

        player.ID.playerEvents.OnUpdateCameraSettings -= UpdateSettingsAfectedByAim;
        player.ID.playerEvents.OnChangeCameraRange -= ChangeCameraRange;
        player.ID.playerEvents.OnResetCameraRange -= ResetCameraRange;

        gameManager.OnUpdateControllerSensitivity -= UpdateControllerSensitivity;
        gameManager.OnUpdateInvertY -= UpdateInvertY;
    }

    #endregion

    private void Update()
    {
        // camera field of view transition
        playerCamera.m_Lens.FieldOfView = Mathf.Lerp(playerCamera.m_Lens.FieldOfView, currentCameraFOV, Time.deltaTime * transitionSpeedFOV);
    }

    private void LateUpdate()
    {
        if (Time.timeScale != 0) 
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
    }

    #region Methods

    private void LoadPlayerPrefs()
    {
        UpdateControllerSensitivity();
        UpdateInvertY();
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    #endregion
}
