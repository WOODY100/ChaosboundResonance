using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.Stash
{
    public sealed class StashChestInteractable : MonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField] private StashChestController chestController;

        private GameFlow gameFlow;

        private void Awake()
        {
            ResolveGameFlow();

            if (chestController == null)
            {
                UnityEngine.Debug.LogError(
                    "[StashChestInteractable] StashChestController reference is missing.",
                    this);
            }

            if (gameFlow == null)
            {
                UnityEngine.Debug.LogError(
                    "[StashChestInteractable] GameFlow reference could not be resolved.",
                    this);
            }
        }

        public void Interact(PlayerInteractor interactor)
        {
            if (chestController == null)
                return;

            if (gameFlow == null)
            {
                ResolveGameFlow();

                if (gameFlow == null)
                    return;
            }

            // The stash can only be opened from the normal Playing context.
            if (gameFlow.CurrentContext != GameFlowContext.Playing)
                return;

            // The interaction is only an opening action.
            // Closing is handled by the stash window itself.
            if (chestController.IsOpen)
                return;

            // Request the Inventory context.
            gameFlow.Request(GameFlowContext.Inventory);

            // Open the physical chest only if the transition succeeded.
            if (gameFlow.CurrentContext == GameFlowContext.Inventory)
            {
                chestController.Open();
            }
        }

        private void ResolveGameFlow()
        {
            BootstrapContext bootstrapContext =
                BootstrapContext.Current;

            if (bootstrapContext == null)
                return;

            gameFlow =
                bootstrapContext.GameFlow;
        }
    }
}