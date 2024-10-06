using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongCrystalAttackStrategy<T> : IAttackStrategy where T : MonoBehaviour
{
    private Transform ownerTransform;
    private GameObject attackPrefab;
    private Transform attackPoint;
    private PlayerDetector playerDetector;

    public StrongCrystalAttackStrategy(Transform ownerTransform, Animator animator, GameObject attackPrefab, PlayerDetector playerDetector, Transform attackPoint)
    {
        this.ownerTransform = ownerTransform;
        this.attackPrefab = attackPrefab;
        this.attackPoint = attackPoint;
        this.playerDetector = playerDetector;
        ObjectPooler.Instance.CreatePool(attackPrefab);
    }

    public void Execute()
    {
        Vector3 spawnPosition = playerDetector.Player.position + Vector3.up * 5;
        GameObject attackObject = ObjectPooler.Instance.Spawn(attackPrefab, spawnPosition, Quaternion.identity);
        if (attackObject != null)
        {
            T attack = attackObject.GetComponent<T>();
            if (attack != null)
            {
                (attack as IDistanceAttack).Initialize(Vector3.down, ownerTransform.gameObject);
            }
        }
    }
}
