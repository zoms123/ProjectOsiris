using UnityEngine;

public class BasicGravityAttackStrategy<T> : IAttackStrategy where T : MonoBehaviour
{
    private Transform ownerTransform;
    private GameObject attackPrefab;
    private Transform attackPoint;
    private PlayerDetector playerDetector;

    public BasicGravityAttackStrategy(Transform ownerTransform, Animator animator, GameObject attackPrefab, PlayerDetector playerDetector, Transform attackPoint)
    {
        this.ownerTransform = ownerTransform;
        this.attackPrefab = attackPrefab;
        this.attackPoint = attackPoint;
        this.playerDetector = playerDetector;
        ObjectPooler.Instance.CreatePool(attackPrefab);
    }

    public void Execute()
    {
        Vector3 relativePos = playerDetector.Player.position - ownerTransform.position;
        relativePos.y = 0;

        GameObject attackObject = ObjectPooler.Instance.Spawn(attackPrefab, attackPoint.position, Quaternion.LookRotation(relativePos * -1, Vector3.up));
       if (attackObject != null)
        {
            T attack = attackObject.GetComponent<T>();
            if (attack != null)
            {
                (attack as IDistanceAttack).Initialize(attackPoint.forward, ownerTransform.tag);
            }
        }
    }
}