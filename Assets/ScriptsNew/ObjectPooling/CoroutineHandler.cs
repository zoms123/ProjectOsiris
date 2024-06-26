using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHandler : MonoBehaviour
{
    private static CoroutineHandler instance;
    public static CoroutineHandler Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new("CoroutineHandler");
                instance = obj.AddComponent<CoroutineHandler>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        return base.StartCoroutine(routine);
    }

    public new void StopCoroutine(Coroutine coroutine)
    {
        base.StopCoroutine(coroutine);
    }

    public new void StopAllCoroutines()
    {
        base.StopAllCoroutines();
    }
}
