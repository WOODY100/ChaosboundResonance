using Chaosbound.Core.Audio;
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
        [SerializeField] private UIAudioPlayer uiAudioPlayer;

        private GameFlow gameFlow;

        private void Awake()
        {
            ResolveGameFlow();
            ResolveUIAudioPlayer();

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

            if (uiAudioPlayer == null)
            {
                UnityEngine.Debug.LogWarning(
                    "[PersonalStashWindowController] UIAudioPlayer reference is missing.",
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

            // Play the UI window-close sound only after a successful close.
            if (uiAudioPlayer != null)
                uiAudioPlayer.PlayPopup();

            // The chest plays its own physical closing sound.
            if (chestController != null)
                chestController.Close();
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

        private void ResolveUIAudioPlayer()
        {
            if (uiAudioPlayer != null)
                return;

            uiAudioPlayer = FindAnyObjectByType<UIAudioPlayer>();
        }
    }
}