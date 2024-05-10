using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravityZone : MonoBehaviour
{
    [SerializeField] float activeTime;

    private List<ZeroGravityEffector> zeroGravityEffectors;

    private void Awake()
    {
        zeroGravityEffectors = new List<ZeroGravityEffector>();
    }

    private void OnEnable()
    {
        if (activeTime > 0)
            StartCoroutine(DisableZoneCountDown());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        foreach(ZeroGravityEffector effector in zeroGravityEffectors)
        {
            effector.StopUsingZeroGravity();
        }
        zeroGravityEffectors.Clear();
    }

    #region Coroutines
    private IEnumerator DisableZoneCountDown()
    {
        yield return new WaitForSeconds(activeTime);
        gameObject.SetActive(false);
    }
    #endregion

    #region Collisions and triggers

    private void OnTriggerEnter(Collider other)
    {
        ZeroGravityEffector effector = other.GetComponent<ZeroGravityEffector>();
        if (effector)
        {
            zeroGravityEffectors.Add(effector);
            effector.UseZeroGravity();
        }
    }

    #endregion
}
