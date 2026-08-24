using UnityEngine;

public sealed class PlayerInteractor : MonoBehaviour
{
    private IInteractable currentInteractable;

    public bool CanInteract => currentInteractable != null;

    private void OnDisable()
    {
        currentInteractable = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();

        if (interactable == null)
        {
            return;
        }

        if (currentInteractable == interactable)
        {
            return;
        }

        currentInteractable = interactable;
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponentInParent<IInteractable>();

        if (interactable == null)
        {
            return;
        }

        if (currentInteractable != interactable)
        {
            return;
        }

        currentInteractable = null;
    }

    public void TryInteract()
    {
        if (!CanInteract)
        {
            return;
        }

        currentInteractable.Interact(this);
    }
}