using System;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    [SerializeField] private List<InteractableBase> actions;
    [SerializeField] private List<GameObject> conditions;


    private bool actionsActivated;

    private void OnEnable()
    {
        foreach (GameObject  conditionObject in conditions)
        {
            IActivable activable = conditionObject.GetComponent<IActivable>();
            activable.OnActivated += Execute;
            activable.OnDeactivated += Execute;
        }
    }

    private void OnDisable()
    {
        foreach (GameObject conditionObject in conditions)
        {
            IActivable activable = conditionObject.GetComponent<IActivable>();
            activable.OnActivated -= Execute;
            activable.OnDeactivated -= Execute;
        }
    }

    private bool ComputeConditions()
    {
        bool contidionInitialized = false;
        bool result = false;
        foreach(GameObject conditionObject in conditions)
        {
            IInteractable interactable = conditionObject.GetComponent<IInteractable>();
            if (!contidionInitialized)
            {
                result = interactable.Activated();
                contidionInitialized = true;
            }
            else
            {
                result = result && interactable.Activated();
            }
        }
        return result;
    }
    private void Execute()
    {
        if (ComputeConditions())
        {
            ExecuteActions();
            actionsActivated = true;
        } else if (actionsActivated)
        {
            ExecuteActions();
            actionsActivated = false;
        }
    }

    private void ExecuteActions()
    {
        foreach (IInteractable interactable in actions)
        {
            interactable.Interact();
        }
    }
}