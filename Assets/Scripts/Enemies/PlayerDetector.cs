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

/*
 using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] float sphereRadius;
    [SerializeField] LayerMask layer;
    [SerializeField] string playerTag = "Player";
    [SerializeField] float maxTimeWithoutDetection;
 
    private IDetectionStrategy detectionStrategy;
    private bool isPlayerNoLongerDetected;
    private float currentTimeWithoutDetection;

    private Transform player;
    public Transform Player { get { return player; } }
    
    private void Awake()
    {
        detectionStrategy = new SphereDetectionStrategy(transform, sphereRadius, layer, playerTag);
    }

    private void Update()
    {

    }

    public bool CanDetectPlayer()
    {
        if (isPlayerNoLongerDetected)
        {
            currentTimeWithoutDetection += Time.deltaTime;
            if (currentTimeWithoutDetection >= maxTimeWithoutDetection)
            {
                isPlayerNoLongerDetected = false;
                player = null;
                currentTimeWithoutDetection = 0;
            }
            return isPlayerNoLongerDetected;
        }


        var previouslyDetectedPlayer = player != null ? player : null;
        var isPlayerDetected = detectionStrategy.Execute(out player);
        
        if(!isPlayerDetected && previouslyDetectedPlayer != null)
        {
            Debug.Log("Player lost");
            player = previouslyDetectedPlayer;
            isPlayerNoLongerDetected = true;
        }

        return isPlayerDetected ? isPlayerDetected : isPlayerNoLongerDetected;
    } 

    public float PlayerDistance(Vector3 position) => Vector3.Distance(position, Player.position);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
 
 */