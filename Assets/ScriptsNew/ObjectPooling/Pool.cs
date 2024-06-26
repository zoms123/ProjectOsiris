using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pool
{
    private readonly GameObject parentObject;
    private readonly Queue<GameObject> pooledObjects = new();
    private readonly GameObject prefab;

    public Pool(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;
        parentObject = new GameObject(prefab.name + "_POOL");

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = CreateNewObject();
            obj.SetActive(false);
            obj.transform.SetParent(parentObject.transform);
            pooledObjects.Enqueue(obj);
        }
    }

    public GameObject Spawn(Vector3 position = default, Quaternion rotation = default)
    {
        GameObject obj;
        if (pooledObjects.Count > 0)
        {
            obj = pooledObjects.Dequeue();
            obj.transform.SetParent(null);
            obj.SetActive(true);
        }
        else
        {
            obj = CreateNewObject();
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        return obj;
    }

    private GameObject CreateNewObject()
    {
        GameObject obj = Object.Instantiate(prefab);
        obj.name = prefab.name;
        return obj;
    }

    public void Despawn(GameObject obj, float despawnDelay = 0f)
    {
        CoroutineHandler.Instance.StartCoroutine(DelayedDespawn(obj, despawnDelay));
    }

    private IEnumerator DelayedDespawn(GameObject obj, float despawnDelay)
    {
        yield return new WaitForSeconds(despawnDelay);

        if (obj != null)
        {
            obj.SetActive(false);
            obj.transform.SetParent(parentObject.transform);
            pooledObjects.Enqueue(obj);
        }
        else
        {
            Debug.LogWarning("Trying to despawn a null object.");
        }
    }
}
