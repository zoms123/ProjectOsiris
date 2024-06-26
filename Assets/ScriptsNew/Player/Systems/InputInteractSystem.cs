using UnityEngine;

public class InputInteractSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Required] private InputManagerSO inputManager;

    [Header("Intercat Settings")]
    [SerializeField, Required] private Transform point;
    [SerializeField] private float radius;

    #region Events

    private void OnEnable()
    {
        inputManager.OnInteract += Interact;
    }

    public void Interact()
    {
        Collider[] collidersTouched = Physics.OverlapSphere(point.position, radius);
        foreach (Collider collider in collidersTouched)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.CanInteract(PowerType.None))
            {
                interactable.Interact();
                if (!interactable.Activated())
                {
                    interactable = null;
                }
                break;
            }
        }
    }

    private void OnDisable()
    {
        inputManager.OnInteract -= Interact;
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(point.position, radius);
    }

    #endregion
}
