using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] float sphereRadius;
    [SerializeField] LayerMask layer;
    [SerializeField] string playerTag = "Player";
 
    private IDetectionStrategy detectionStrategy;

    private Transform player;
    public Transform Player { get { return player; } }
    
    private void Awake()
    {
        detectionStrategy = new SphereDetectionStrategy(transform, sphereRadius, layer, playerTag);
    }

    private void Update()
    {
        // TODO Configure timer to not call the estrategy each frame.
    }

    public bool CanDetectPlayer() => detectionStrategy.Execute(out player);

    public float PlayerDistance(Vector3 position) => Vector3.Distance(position, Player.position);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}