using System;
using System.Collections;
using UnityEngine;

public class CrystalDissolveEffect : MonoBehaviour, IInteractable
{
    [SerializeField] private Material material;
    [SerializeField] private float effectTime;
    [SerializeField] private Collider colliderToDisolve;
    [SerializeField] private Renderer rendererToDisolve;
    [SerializeField] private bool ReapearWhenDisolved;
    [SerializeField] private bool canBeDisolved;

    [Header("Timer")]
    [SerializeField] private bool activateTimer;
    [SerializeField] private float activeTime;
 
    private float total = 1;
    private Material copyMaterial;
    private bool hidden = true;
    private float currentTime;
    private bool applyEffect;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform transformToDissolve;
    private bool timerActivated;

    void Start()
    {
        copyMaterial = new Material(material);
        rendererToDisolve.material = copyMaterial;
        copyMaterial.SetFloat("_Dissolve", total);
        colliderToDisolve.enabled = !hidden;
        transformToDissolve = rendererToDisolve.transform;
        originalPosition = transformToDissolve.position;
        originalRotation = transform.rotation;
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
            if (hidden && transformToDissolve.position != originalPosition)
            {
                Debug.Log("returning to origin " + transformToDissolve.position + " " + originalPosition);
                transformToDissolve.parent = transform;
                transformToDissolve.position = originalPosition;
                transformToDissolve.rotation = originalRotation;
                if (ReapearWhenDisolved)
                {
                    ActivateAppearingAndDisappearingEffect();
                }
            } else if(!hidden && activateTimer)
            {
                timerActivated = true;
                StartCoroutine(Timer());
                
            }
        }
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(activeTime);
        timerActivated = false;
        ActivateAppearingAndDisappearingEffect();
        
    }

    private float CalculateEffectRatio()
    {
        var ratio = currentTime / effectTime;
        return hidden ? ratio : -ratio;
        
    }

    private void ActivateAppearingAndDisappearingEffect()
    {
        if ((!hidden && transformToDissolve.parent == transform && ReapearWhenDisolved) ||
            (!hidden && !canBeDisolved && !activateTimer) ||
            (activateTimer && timerActivated)
            )
        {
            return;
        }
            
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
