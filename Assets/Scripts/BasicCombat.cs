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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position, radius);
    }
}
