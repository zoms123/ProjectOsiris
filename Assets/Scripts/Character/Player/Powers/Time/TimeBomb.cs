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
        visual = transform.GetChild(0).gameObject;
        visual.SetActive(true);
        if (waitTime > 0 && radius > 0)
        {
            for (int i = 0; i < visual.transform.childCount; i++)
            {
                float newScale = radius * 1.15f;
                visual.transform.GetChild(i).localScale = new Vector3(newScale, newScale, newScale);
            }
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
                StartCoroutine(lifesystem.ReceiveDamage(damage));
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
