using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class CrystalDissolveEffect : MonoBehaviour, IInteractable
{
    [SerializeField] Material material;
    [SerializeField] float effectTime;
    [SerializeField] float appearingRatio = 0.1f;
    [SerializeField] float speed = 2;

    private bool canInteract = true; 
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
        if (applyEffect && currentTime <= effectTime)
        {
            currentTime += Time.deltaTime;
            float effectRatio = CalculateEffectRatio();          
            copyMaterial.SetFloat("_Dissolve", total + effectRatio);
        } 
        else
        {
            applyEffect = false;
            currentTime = 0;
            total = hidden ? 1 : 0;
        }
    }


    private float CalculateEffectRatio()
    {
        var ratio = currentTime / effectTime;
        return hidden ? ratio : -ratio;
        
    }

    private void ActivateAppearingAndDisappearingEffect()
    {
        hidden = !hidden;
        applyEffect = true;
        
        // Activate or deactivate colliders
    }

    public bool CanInteract()
    {
        return canInteract;
    }

    public void Interact(PowerType powerType)
    {
        if (powerType == PowerType.Crystal && !applyEffect)
        {
            ActivateAppearingAndDisappearingEffect();
        }
    }
}
