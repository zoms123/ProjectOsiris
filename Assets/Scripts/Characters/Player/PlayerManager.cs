using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PowerType currentPowerType = PowerType.None;
    [SerializeField] private RawImage powerIcon;

    [Header("Textures")]
    [SerializeField] private Texture2D gravity;
    [SerializeField] private Texture2D crystal;
    [SerializeField] private Texture2D time;
    [SerializeField] private Texture2D shadow;

    [Header("Inputs")]

    [SerializeField, Required] private InputManagerSO inputManager;

    // Scripts
    private PlayerLocomotion playerlocomotion;
    private PlayerAiming playerAiming;
    private BasicCombat basicCombat;

    public PowerType CurrentPowerType { get { return currentPowerType; } }
    public PlayerLocomotion Locomotion { get { return playerlocomotion; } }
    public PlayerAiming Aiming { get { return playerAiming; } }

    protected void Awake()
    {
        playerlocomotion = GetComponent<PlayerLocomotion>();
        playerAiming = GetComponent<PlayerAiming>();
        basicCombat = GetComponent<BasicCombat>();
    }

    private void Update()
    {
        // Handle movement
        playerlocomotion.HandleAllMovement();
    }

    private void OnEnable()
    {
        inputManager.OnPowerSelect += OnPowerSelected;
        inputManager.OnAttack += ExecuteAttack;
    }

    private void OnDisable()
    {
        inputManager.OnPowerSelect -= OnPowerSelected;
        inputManager.OnAttack -= ExecuteAttack;
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

    private void ExecuteAttack()
    {
        if (playerlocomotion.isGrounded)
        {
            basicCombat.Attack();
        }
    }
}
