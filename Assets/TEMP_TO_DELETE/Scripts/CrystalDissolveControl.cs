using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalDissolveControl : MonoBehaviour
{
    [SerializeField] Material mat;

    float total = 1;
    private Material copyMaterial;

    void Start()
    {
        copyMaterial = new Material(mat);
        GetComponent<Renderer>().material = copyMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            total -= 0.1f;
            copyMaterial.SetFloat("_Dissolve", total);
        }
    }
}
