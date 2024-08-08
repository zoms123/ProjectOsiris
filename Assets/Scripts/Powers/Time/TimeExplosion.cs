using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeExplosion : MeleeAttack
{
    [SerializeField] float damage;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private float growthSpeed;
    [SerializeField] private LayerMask whatIsDamageable;

    private void OnEnable()
    {
        if (maxScale > 0 && minScale > 0)
        {
            gameObject.transform.localScale = new Vector3(minScale, minScale, minScale);
        }
    }

    protected override void PerformAttack()
    {
        // Calcula el nuevo tama�o bas�ndote en la velocidad de crecimiento
        float newScale = transform.localScale.x + growthSpeed * Time.deltaTime;

        // Limita el tama�o m�ximo para que no se pase y explota al llegar al m�ximo
        if (newScale > maxScale)
        {
            newScale = maxScale;
            transform.localScale = new Vector3(newScale, newScale, newScale);
            Explode();
            ReturnToPool();
        }

        // Asigna el nuevo tama�o al objeto
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    private void Explode()
    {
        Collider[] collidersTouched = Physics.OverlapSphere(transform.position, maxScale/2, whatIsDamageable);
        foreach (Collider collider in collidersTouched)
        {
            if (!collider.CompareTag("Enemy"))
            { // Temp Check
                Debug.Log("ExplosionHit");
                LifeSystem lifesystem = collider.gameObject.GetComponent<LifeSystem>();
                lifesystem.ReceiveDamage(damage);
            }
        }
    }

    protected void ReturnToPool()
    {
        initialized = false;
        ObjectPooler.Instance.Despawn(gameObject);
    }

    #region Debug

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 center = transform.position;
        center.y += maxScale;
        Gizmos.DrawWireSphere(center, maxScale*1.4f);
    }

    #endregion
}
