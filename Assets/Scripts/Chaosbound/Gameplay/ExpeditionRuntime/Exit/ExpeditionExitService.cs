using Chaosbound.Core.GameFlow;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Gameplay.ExpeditionRuntime.Director;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Gameplay.ExpeditionRuntime.Settlement;
using Chaosbound.Gameplay.Save;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Exit
{
    /// <summary>
    /// Coordinates the terminal exit of an active Expedition.
    ///
    /// On successful completion, this service attempts to settle
    /// the Expedition results before cleaning up the runtime.
    ///
    /// If settlement fails, no partial settlement is performed.
    /// The failure is logged and the Expedition is still safely
    /// aborted before returning to Sanctuary.
    ///
    /// This service does not own Expedition runtime state,
    /// cleanup logic or GameFlow state.
    /// </summary>
    public sealed class ExpeditionExitService
    {
        private readonly ExpeditionDirector expeditionDirector;
        private readonly GameFlow gameFlow;
        private readonly SceneTransitionService sceneTransitionService;

        private readonly ExpeditionSettlementService settlementService;

        private readonly ExpeditionSecurePreservationService
            securePreservationService;

        private readonly IPersistentStateSaver persistentStateSaver;

        public ExpeditionExitService(
            ExpeditionDirector expeditionDirector,
            ExpeditionSettlementService settlementService,
            ExpeditionSecurePreservationService securePreservationService,
            GameFlow gameFlow,
            SceneTransitionService sceneTransitionService,
            IPersistentStateSaver persistentStateSaver)
        {
            this.expeditionDirector =
                expeditionDirector
                ?? throw new ArgumentNullException(
                    nameof(expeditionDirector));

            this.settlementService =
                settlementService
                ?? throw new ArgumentNullException(
                    nameof(settlementService));

            this.securePreservationService =
                securePreservationService
                ?? throw new ArgumentNullException(
                    nameof(securePreservationService));

            this.gameFlow =
                gameFlow
                ?? throw new ArgumentNullException(
                    nameof(gameFlow));

            this.sceneTransitionService =
                sceneTransitionService
                ?? throw new ArgumentNullException(
                    nameof(sceneTransitionService));

            this.persistentStateSaver =
                persistentStateSaver
                ?? throw new ArgumentNullException(
                    nameof(persistentStateSaver));
        }

        public void Exit(
            ExpeditionExitReason reason,
            RuntimeExpeditionConfig expeditionConfig)
        {
            if (reason == ExpeditionExitReason.Completed)
            {
                bool settlementSucceeded =
                    settlementService.TrySettle(
                        expeditionDirector.RuntimeState,
                        expeditionConfig,
                        out ExpeditionSettlementResult result);

                if (!settlementSucceeded)
                {
                    UnityEngine.Debug.LogError(
                        "[ExpeditionExitService] " +
                        "Expedition completion settlement failed. " +
                        "No persistent progress was committed.");
                }
                else
                {
                    persistentStateSaver.SavePersistentState();
                }
            }
            else
            {
                bool preservationSucceeded =
                    securePreservationService.TryPreserve();

                if (!preservationSucceeded)
                {
                    UnityEngine.Debug.LogError(
                        "[ExpeditionExitService] " +
                        "Secure Inventory preservation failed.");
                }
                else
                {
                    persistentStateSaver.SavePersistentState();
                }
            }

            AbortExpedition();

            gameFlow.ResetFlow();

            gameFlow.SetEnvironment(
                GameFlowEnvironment.Sanctuary);

            sceneTransitionService.LoadScene(
                GameScene.Sanctuary);

            gameFlow.Initialize();
        }

        private void AbortExpedition()
        {
            expeditionDirector.AbortExpedition();
        }
    }
}