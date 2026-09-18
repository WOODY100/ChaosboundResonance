using Chaosbound.Core.Composition;
using Chaosbound.Core.GameFlow;
using Chaosbound.Gameplay.Inventory.Stash;
using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class PersonalStashWindowController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StashChestController chestController;

        private GameFlow gameFlow;

        private void Awake()
        {
            ResolveGameFlow();

            if (gameFlow == null)
            {
                UnityEngine.Debug.LogError(
                    "[PersonalStashWindowController] GameFlow reference could not be resolved.",
                    this);
            }

            if (chestController == null)
            {
                UnityEngine.Debug.LogError(
                    "[PersonalStashWindowController] StashChestController reference is missing.",
                    this);
            }
        }

        public void Close()
        {
            if (gameFlow == null)
            {
                ResolveGameFlow();

                if (gameFlow == null)
                    return;
            }

            if (gameFlow.CurrentContext != GameFlowContext.Inventory)
                return;

            bool closed =
                gameFlow.Pop(GameFlowContext.Inventory);

            if (!closed)
                return;

            if (chestController != null)
            {
                chestController.Close();
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