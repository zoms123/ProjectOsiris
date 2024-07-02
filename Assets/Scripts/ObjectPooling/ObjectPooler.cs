using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler
{
    private static ObjectPooler instance;
    public static ObjectPooler Instance
    {
        get
        {
            instance ??= new();
            return instance;
        }
    }

    private readonly Dictionary<string, Pool> pools = new();
    private readonly Dictionary<string, GameObject> originalPrefabs = new();

    public void CreatePool(GameObject prefab, int initialSize = 1)
    {
        string key = prefab.name;
        if (!pools.ContainsKey(key))
        {
            pools[key] = new(prefab, initialSize);
            originalPrefabs[key] = prefab;
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position = default, Quaternion rotation = default)
    {
        string key = prefab.name;

        if (!pools.ContainsKey(key)) CreatePool(prefab);

        return pools[key].Spawn(position, rotation);
    }

    public void Despawn(GameObject obj, float despawnDelay = 0f)
    {
        string key = obj.name.Replace("(Clone)", "").Trim();

        if (pools.ContainsKey(key))
        {
            pools[key].Despawn(obj, despawnDelay);
        }
        else
        {
            if (originalPrefabs.ContainsKey(key))
            {
                GameObject originalPrefab = originalPrefabs[key];
                CreatePool(originalPrefab);
                pools[key].Despawn(obj, despawnDelay);
            }
            else
            {
                GameObject.Destroy(obj, despawnDelay);
            }
        }
    }
}
