using UnityEngine;

public class BasicCrystalAttackStrategy<T> : IAttackStrategy where T : MonoBehaviour
{
    private Transform spawnPosition;
    private Transform currentTarget;
    private Transform playerTransform;
    private Transform ownerTransform;
    private Animator animator;
    private GameObject attackPrefab;
    private PlayerDetector playerDetector;
    private Vector3 attackOffset;

    

    public BasicCrystalAttackStrategy(Transform ownerTransform, Animator animator, GameObject attackPrefab, PlayerDetector playerDetector, Transform spawnPosition)
    {
        this.animator = animator;
        this.ownerTransform = ownerTransform;
        this.attackPrefab = attackPrefab;
        this.playerDetector = playerDetector;
        ObjectPooler.Instance.CreatePool(attackPrefab);
        this.spawnPosition = spawnPosition;
    }

    public void Execute()
    {
        playerTransform = playerDetector.Player;

        Vector3 relativePos = playerDetector.Player.position - ownerTransform.position;
        relativePos.y = 0;

        GameObject attackObject = ObjectPooler.Instance.Spawn(attackPrefab, spawnPosition.position, Quaternion.LookRotation(relativePos * -1, Vector3.up));
        
        if (attackObject != null)
        {
            T attack = attackObject.GetComponent<T>();
            (attack as IDistanceAttack).Initialize(playerTransform.position, ownerTransform.gameObject);
        }
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
}