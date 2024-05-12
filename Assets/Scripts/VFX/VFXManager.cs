using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXManager : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    private GameObject instance;
    private VisualEffect visualEffect;
    //private float lifeTime;
    //private float currentTime;

    //private bool execute;

 
    public void ExecuteVFX(GameObject prefab)
    {
        //execute = true;
        instance = ObjectPool.GetObject(prefab);
        if (!instance)
        {
            ObjectPool.Initialize(prefab);
            instance = ObjectPool.GetObject(prefab);
        }
        instance.transform.parent = spawnPoint;
        instance.transform.position = spawnPoint.position;
        instance.transform.rotation = spawnPoint.rotation;
        visualEffect = instance.GetComponentInChildren<VisualEffect>();
        instance.SetActive(true);
        visualEffect.Play();
    }

    private void Update()
    {
       /* if (execute && visualEffect.isActiveAndEnabled)
        {
            Debug.Log("active");
            currentTime += Time.deltaTime;
            if(currentTime >= lifeTime)
            {
             //   visualEffect.Stop();
            //    execute = false;
           //     ObjectPool.ReturnObject(instance);
            }
        }
        else if (execute && !visualEffect.isActiveAndEnabled)
        {
            Debug.Log("No active");
            ObjectPool.ReturnObject(instance);
            execute = false;
        }
        */
    }

    public void StopVFX()
    {
        visualEffect.Stop();
        //execute = false;
        ObjectPool.ReturnObject(instance);
    }
}
