using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class TimeBomb : MonoBehaviour
{
    [SerializeField] float waitTime;
    [SerializeField] float damage;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask whatIsDamageable;

    private GameObject visual;

    private void OnEnable()
    {        
        if (waitTime > 0 && radius > 0)
        {
            visual = transform.GetChild(0).gameObject;
            visual.SetActive(true);
            visual.transform.localScale = new Vector3(radius, radius, radius);
            StartCoroutine(WaitExplodeBomb());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Explode()
    {
        Collider[] collidersTouched = Physics.OverlapSphere(transform.position, radius, whatIsDamageable);
        foreach (Collider collider in collidersTouched)
        {
            if (!collider.CompareTag("Player")) { // Temp Check
                Debug.Log("BombHit");
                LifeSystem lifesystem = collider.gameObject.GetComponent<LifeSystem>();
                lifesystem.ReceiveDamage(damage);
            }
        }
    }

    #region Coroutines
    private IEnumerator WaitExplodeBomb()
    {
        yield return new WaitForSeconds(waitTime);
        Explode();
        visual.SetActive(false);
        yield return new WaitForSeconds(0.7f);
        gameObject.SetActive(false);
    }
    #endregion

    #region Debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    #endregion
}
