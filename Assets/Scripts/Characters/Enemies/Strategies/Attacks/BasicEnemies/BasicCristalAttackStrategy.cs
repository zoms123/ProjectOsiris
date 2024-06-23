using UnityEngine;

public class BasicCrystalAttackStrategy<T> : IAttackStrategy where T : MonoBehaviour
{
    private Transform currentTarget;
    private Transform playerTransform;
    private Transform ownerTransform;
    private Animator animator;
    private GameObject attackPrefab;
    private PlayerDetector playerDetector;

    public BasicCrystalAttackStrategy(Transform ownerTransform, Animator animator, GameObject attackPrefab, PlayerDetector playerDetector)
    {
        this.animator = animator;
        this.ownerTransform = ownerTransform;
        this.attackPrefab = attackPrefab;
        this.playerDetector = playerDetector;
        this.playerTransform = playerDetector.Player;
        ObjectPool.Initialize(attackPrefab);
    }

    public void Execute()
    {
        Debug.Log("prefab " + attackPrefab);
        GameObject attackObject = ObjectPool.GetObject(attackPrefab);
        attackObject.SetActive(false);
        Debug.Log("attackObject " + attackObject);
        if (attackObject != null)
        {
            Debug.Log(playerTransform);
            attackObject.transform.position = playerTransform.position;
            attackObject.transform.rotation = playerTransform.rotation;
            T attack = attackObject.GetComponent<T>();
            (attack as ITrapAttack).Initialize(ownerTransform.tag);
        }
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
    }
}