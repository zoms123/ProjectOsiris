using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeroGravityZone : MonoBehaviour
{
    [SerializeField] float activeTime;

    private List<IZeroGravityEffector> zeroGravityEffectors;

    private void Awake()
    {
        zeroGravityEffectors = new List<IZeroGravityEffector>();
    }

    private void OnEnable()
    {
        if (activeTime > 0)
            StartCoroutine(DisableZoneCountDown());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        foreach(IZeroGravityEffector effector in zeroGravityEffectors)
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
        IZeroGravityEffector effector = other.GetComponent<IZeroGravityEffector>();
        if (effector != null)
        {
            zeroGravityEffectors.Add(effector);
            effector.UseZeroGravity();
        }
    }

    #endregion
}
