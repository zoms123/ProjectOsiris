using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform target;

    private void OnEnable()
    {
        if (target == null) target = Camera.main.transform;
        StartCoroutine(LookAtTarget());
    }

    private IEnumerator LookAtTarget()
    {
        while (this.gameObject.activeInHierarchy)
        {
            Vector3 dir = target.position - this.transform.position;

            transform.rotation = Quaternion.LookRotation(dir);
            yield return null;
        }
    }
}
