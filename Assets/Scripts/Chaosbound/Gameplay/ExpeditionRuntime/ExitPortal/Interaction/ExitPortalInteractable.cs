using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Runtime;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal
{
    /// <summary>
    /// Receives player interaction with the physical
    /// Exit Portal and forwards the request to the
    /// Exit Portal runtime state.
    /// </summary>
    public sealed class ExitPortalInteractable :
        UnityEngine.MonoBehaviour,
        IInteractable
    {
        private ExitPortalRuntimeState runtimeState;

        /// <summary>
        /// Initializes the interactable with the runtime
        /// state owned by the current expedition.
        /// </summary>
        public void Initialize(
            ExitPortalRuntimeState runtimeState)
        {
            this.runtimeState =
                runtimeState
                ?? throw new ArgumentNullException(
                    nameof(runtimeState));
        }

        /// <inheritdoc/>
        public void Interact(
            PlayerInteractor interactor)
        {
            if (interactor == null)
            {
                throw new ArgumentNullException(
                    nameof(interactor));
            }

            if (runtimeState == null)
            {
                throw new InvalidOperationException(
                    "ExitPortalInteractable has not been initialized.");
            }

            if (runtimeState.State !=
                ExitPortalDomainState.Spawned)
            {
                return;
            }

            runtimeState.RequestInteraction();
        }
    }
}