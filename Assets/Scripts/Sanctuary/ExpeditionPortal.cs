using Chaosbound.Runtime.Bootstrap;
using UnityEngine;

public sealed class ExpeditionPortal : MonoBehaviour, IInteractable
{
    [SerializeField]
    private ExpeditionBootstrap expeditionBootstrap;

    public void Interact(PlayerInteractor interactor)
    {
        expeditionBootstrap.StartExpedition();
    }
}