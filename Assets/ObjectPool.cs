using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectPool 
{

    private static Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();

    private static int pooledAmountPerObject = 10;

    public static void Initialize(GameObject prefab)
    {
        List<GameObject> objects = new List<GameObject>();
        for (int i = 0; i < pooledAmountPerObject; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            obj.SetActive(false);
            objects.Add(obj);
        }
        pooledObjects.Add(prefab, objects);
    }

    public static GameObject GetObject(GameObject prefab)
    {
        if (pooledObjects.ContainsKey(prefab))
        {
            foreach (GameObject obj in pooledObjects[prefab])
            {
                if (!obj.activeInHierarchy)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }
            GameObject newObj = GameObject.Instantiate(prefab);
            newObj.SetActive(true);
            pooledObjects[prefab].Add(newObj);
            return newObj;
        }
        Debug.LogWarning("No available objects in pool for prefab " + prefab.name);
        return null;
    }

    public static void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
