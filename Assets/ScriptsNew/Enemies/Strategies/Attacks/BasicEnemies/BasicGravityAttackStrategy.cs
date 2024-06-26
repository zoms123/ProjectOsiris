using UnityEngine;

public class BasicGravityAttackStrategy<T> : IAttackStrategy where T : MonoBehaviour
{
    private Transform currentTarget;
    private Transform ownerTransform;
    private Animator animator;
    private GameObject attackPrefab;
    private Transform attackPoint;
    private Transform spawnPoint;
    private PlayerDetector playerDetector;
    private Transform playerTransform;

    public BasicGravityAttackStrategy(Transform ownerTransform, Animator animator, GameObject attackPrefab, PlayerDetector playerDetector, Transform attackPoint, Transform spawnPoint)
    {
        this.animator = animator;
        this.ownerTransform = ownerTransform;
        this.attackPrefab = attackPrefab;
        this.attackPoint = attackPoint;
        this.spawnPoint = spawnPoint;
        this.playerDetector = playerDetector;
        ObjectPooler.Instance.CreatePool(attackPrefab);
    }

    public void Execute()
    {
        playerTransform = playerDetector.Player;
        Vector3 relativePos = playerDetector.Player.position - ownerTransform.position;
        relativePos.y = 0;

        GameObject attackObject = ObjectPooler.Instance.Spawn(attackPrefab, attackPoint.position, Quaternion.LookRotation(relativePos * -1, Vector3.up));
        attackObject.transform.parent = attackPoint;
       if (attackObject != null)
        {
            T attack = attackObject.GetComponent<T>();
            if (attack != null)
            {
                (attack as IDistanceAttack).Initialize(spawnPoint.forward, ownerTransform.tag, spawnPoint);
            }
        }
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
}