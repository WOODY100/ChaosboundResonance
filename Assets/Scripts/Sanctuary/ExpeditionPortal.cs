
using Chaosbound.Gameplay.Expeditions.UI;
using UnityEngine;

public sealed class ExpeditionPortal : MonoBehaviour, IInteractable
{
    [Header("Expedition Selection")]
    [SerializeField]
    private ExpeditionSelectionWindowController selectionWindowController;

    public void Interact(PlayerInteractor interactor)
    {
        if (selectionWindowController == null)
        {
            Debug.LogError(
                "[ExpeditionPortal] Missing " +
                "ExpeditionSelectionWindowController reference.",
                this);

            return;
        }

        bool opened = selectionWindowController.Open();

        if (!opened)
        {
            Debug.LogWarning(
                "[ExpeditionPortal] Could not open expedition selection.",
                this);
        }
    }
}
