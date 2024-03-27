using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCombat : MonoBehaviour
{
    [Header("Attack System")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector3 attackExtend;
    [SerializeField] private float attackDamage;
    [SerializeField] private LayerMask whatIsDamageable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DoAttack();
    }

    private void DoAttack()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Attack();
        }
    }

    private void Attack()
    {
        Collider[] collidersTouched = Physics.OverlapBox(attackPoint.position, attackExtend, Quaternion.identity, whatIsDamageable);
        foreach (Collider collider in collidersTouched)
        {
            if (!collider.gameObject.CompareTag("PlayerHitBox"))
            {
                LifeSystem lifesystem = collider.gameObject.GetComponent<LifeSystem>();
                StartCoroutine(lifesystem.ReceiveDamage(attackDamage));
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(attackPoint.position, attackExtend);
    }
}
