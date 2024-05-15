using System;
using UnityEngine;

public class CrystalDissolveEffect : MonoBehaviour, IInteractable
{
    [SerializeField] Material material;
    [SerializeField] float effectTime;
    [SerializeField] Collider colliderToDisolve;
 
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
        colliderToDisolve.enabled = !hidden;
    }

    void Update()
    {
        if (applyEffect && currentTime <= effectTime)
        {
            currentTime += Time.deltaTime;
            float effectRatio = CalculateEffectRatio();          
            copyMaterial.SetFloat("_Dissolve", total + effectRatio);
        } 
        else if(applyEffect)
        {
            applyEffect = false;
            currentTime = 0;
            total = hidden ? 1 : 0;
            colliderToDisolve.enabled = !hidden;            
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
        
    }

    public bool CanInteract(PowerType powerType)
    {
        return powerType == PowerType.Crystal;
    }

    public void Interact()
    {
        if (!applyEffect)
        {
            ActivateAppearingAndDisappearingEffect();
        }
    }

    public bool Activated()
    {
        return applyEffect;
    }
}
