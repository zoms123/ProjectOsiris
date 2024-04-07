using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravityController : MonoBehaviour
{
    [SerializeField] private GameManagerSO gameManager;
    [SerializeField] private InputManagerSO inputManager;
    [SerializeField] GameObject zeroGravityZonePrefab;
    [SerializeField] float zeroGravityZoneOffset;

    private GameObject zeroGravityZone;

    private IInteractable interactable;

    private void OnEnable()
    {
        inputManager.OnInteract += Interact;

    }

    private void OnDisable()
    {
        inputManager.OnInteract -= Interact;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (!zeroGravityZone)
            {
                Vector3 position = transform.position + Vector3.forward * zeroGravityZoneOffset;
                zeroGravityZone = Instantiate(zeroGravityZonePrefab, position, Quaternion.identity);
            }
            else if (zeroGravityZone && !zeroGravityZone.activeSelf)
            {
                Vector3 position = transform.position + Vector3.forward * zeroGravityZoneOffset;
                zeroGravityZone.transform.position = position;
                zeroGravityZone.SetActive(true);
            } 
            else
            {
                zeroGravityZone.SetActive(false);
            }
            
            
        }
    }

    private void Interact()
    {
        if (interactable != null && interactable.CanInteract())
        {
            interactable.Interact(PowerType.Gravity);
        }
    }

    #region Collisions and Triggers

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            IInteractable interact = other.GetComponent<IInteractable>();
            if (interact != null)
            {
                interactable = other.GetComponent<IInteractable>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            interactable = null;
        }
    }
    #endregion
}
