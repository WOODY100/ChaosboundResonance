using UnityEngine;

public sealed class ExpeditionPortal : MonoBehaviour, IInteractable
{
    public void Interact(PlayerInteractor interactor)
    {
        Debug.Log("Expedition Portal Activated");
    }
}