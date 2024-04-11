using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCombat : MonoBehaviour
{
    [Header("Attack System")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackDamage;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask whatIsDamageable;
    [SerializeField] private string myTag;

    #region Components
    [Header("Components")]
    [SerializeField] private InputManagerSO inputManager;
    private PlayerManager player;
    #endregion

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    private void OnEnable()
    {
        inputManager.OnAttack += ExecuteAttack;
    }

    private void ExecuteAttack()
    {
        if (player.isGrounded)
        {
            Attack();
        }
    }

    public void Attack()
    {
        Collider[] collidersTouched = Physics.OverlapSphere(attackPoint.position, radius, whatIsDamageable);
        foreach (Collider collider in collidersTouched)
        {
            if (!collider.gameObject.CompareTag(myTag))
            {
                LifeSystem lifesystem = collider.gameObject.GetComponent<LifeSystem>();
                StartCoroutine(lifesystem.ReceiveDamage(attackDamage));
            }
        }
    }

    private void OnDisable()
    {
        inputManager.OnAttack -= ExecuteAttack;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, radius);
    }
}
