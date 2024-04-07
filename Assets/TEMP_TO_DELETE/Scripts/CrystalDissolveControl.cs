using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class CrystalDissolveControl : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] float effectTime;
    [SerializeField] float appearingRatio = 0.1f;
    [SerializeField] float speed = 2;

    private float total = 1;
    private Material copyMaterial;
    private bool hidden = true;
    private float currentTime;
    private bool applyEffect;

    void Start()
    {
        copyMaterial = new Material(material);
        GetComponent<Renderer>().material = copyMaterial;
        copyMaterial.SetFloat("_Dissolve", total);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.L) && !applyEffect)
        {
            ActivateAppearingAndDisappearingEffect();
        }
        if (applyEffect && currentTime <= effectTime)
        {
            
            currentTime += Time.deltaTime;
            float amount = 0;
            if (hidden)
                amount = currentTime / effectTime;
            else
                amount -= currentTime / effectTime;

            copyMaterial.SetFloat("_Dissolve", total + amount);
        } 
        else
        {
            applyEffect = false;
            currentTime = 0;
            if (hidden)
            {
                total = 1;
            } else
            {
                total = 0;
            }
        }
    }

    #region Coroutines

    private void ActivateAppearingAndDisappearingEffect()
    {
        hidden = !hidden;
        applyEffect = true;
        
        // Activbate or deactivate colliders
    }
    #endregion
}
