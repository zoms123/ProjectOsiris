using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : CharacterManager
{
    [Header("References")]
    [SerializeField] private InputManagerSO inputManager;
    [SerializeField] private PowerType currentPowerType = PowerType.None;
    [SerializeField] private RawImage powerIcon;

    [Header("Textures")]
    [SerializeField] private Texture2D gravity;
    [SerializeField] private Texture2D crystal;
    [SerializeField] private Texture2D time;
    [SerializeField] private Texture2D shadow;

    [HideInInspector] public PlayerAnimatorManager animatorManager;
    private PlayerLocomotion locomotion;

    public PowerType CurrentPowerType { get { return currentPowerType; } }

    protected override void Awake()
    {
        base.Awake();

        locomotion = GetComponent<PlayerLocomotion>();
        animatorManager = GetComponent<PlayerAnimatorManager>();
    }

    protected override void Update()
    {
        base.Update();

        // Handle movement
        locomotion.HandleAllMovement();
    }

    private void OnEnable()
    {
        inputManager.OnPowerSelect += OnPowerSelected;
    }

    private void OnDisable()
    {
        inputManager.OnPowerSelect -= OnPowerSelected;
    }

    private void OnPowerSelected(Vector2 power)
    {
        if (power[1] == -1)
        {
            currentPowerType = PowerType.Gravity;
            powerIcon.texture = gravity;
        }
        else if (power[0] == -1)
        {
            currentPowerType = PowerType.Crystal;
            powerIcon.texture = crystal;
        }
        else if (power[0] == 1)
        {
            currentPowerType = PowerType.Time;
            powerIcon.texture = time;
        }
        else
        {
            currentPowerType = PowerType.Shadow;
            powerIcon.texture = shadow;
        }
    }
}
