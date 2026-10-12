using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.GameFlow;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.Director;
using Chaosbound.Gameplay.ExpeditionRuntime.Settlement;
using Chaosbound.Gameplay.Save;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Exit
{
    public enum ExpeditionExitDestination
    {
        Sanctuary,
        MainMenu
    }

    /// <summary>
    /// Coordinates the terminal exit of an active Expedition.
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
        private readonly ExpeditionProgressRuntime expeditionProgressRuntime;

        private bool exitRequested;

        public ExpeditionExitService(
            ExpeditionDirector expeditionDirector,
            ExpeditionSettlementService settlementService,
            ExpeditionSecurePreservationService securePreservationService,
            GameFlow gameFlow,
            SceneTransitionService sceneTransitionService,
            IPersistentStateSaver persistentStateSaver,
            ExpeditionProgressRuntime expeditionProgressRuntime)
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

            this.expeditionProgressRuntime =
                expeditionProgressRuntime
                ?? throw new ArgumentNullException(
                    nameof(expeditionProgressRuntime));
        }

        public void Exit(
    ExpeditionExitReason reason,
    RuntimeExpeditionConfig expeditionConfig)
        {
            Exit(
                reason,
                expeditionConfig,
                ExpeditionExitDestination.Sanctuary);
        }

        public void Exit(
            ExpeditionExitReason reason,
            RuntimeExpeditionConfig expeditionConfig,
            ExpeditionExitDestination destination)
        {
            if (exitRequested)
            {
                Debug.LogWarning(
                    "[ExpeditionExitService] An exit is already in progress.");
                return;
            }

            exitRequested = true;

            try
            {
                if (reason == ExpeditionExitReason.Completed)
                {
                    HandleCompletedExit(expeditionConfig);
                }
                else
                {
                    HandleNonCompletedExit();
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "[ExpeditionExitService] Exit failed: " + exception);
            }
            finally
            {
                ReturnToDestination(destination);
            }
        }

        private bool HandleCompletedExit(
    RuntimeExpeditionConfig expeditionConfig)
        {
            if (expeditionConfig == null)
            {
                Debug.LogError(
                    "[ExpeditionExitService] Cannot complete an " +
                    "Expedition without a runtime configuration.");

                return false;
            }

            if (expeditionConfig.Identity == null ||
                expeditionConfig.Identity.Id.IsEmpty)
            {
                Debug.LogError(
                    "[ExpeditionExitService] Cannot register completion " +
                    "because the Expedition ID is invalid.");

                return false;
            }

            var runtimeState = expeditionDirector.RuntimeState;

            if (runtimeState == null ||
                runtimeState.Completion == null ||
                runtimeState.Completion.State !=
                    CompletionDomainState.Completed)
            {
                Debug.LogError(
                    "[ExpeditionExitService] Cannot settle an Expedition " +
                    "that has not reached the Completed state.");

                return false;
            }

            string expeditionId = expeditionConfig.Identity.Id.Value;

            bool settlementSucceeded =
                settlementService.TrySettle(
                    runtimeState,
                    expeditionConfig,
                    out ExpeditionSettlementResult result);

            if (!settlementSucceeded ||
                result == null ||
                !result.Success)
            {
                Debug.LogError(
                    "[ExpeditionExitService] Expedition settlement failed. " +
                    "Completion was not registered.");

                return false;
            }

            bool completionRegistered =
                expeditionProgressRuntime.TryRegisterCompletion(
                    expeditionId);

            if (!completionRegistered &&
                !expeditionProgressRuntime.HasCompleted(expeditionId))
            {
                Debug.LogError(
                    "[ExpeditionExitService] Settlement succeeded, but " +
                    "the completion record could not be registered for " +
                    expeditionId + ".");

                return false;
            }

            return TrySavePersistentState("completed Expedition");
        }

        private bool HandleNonCompletedExit()
        {
            bool preservationSucceeded =
                securePreservationService.TryPreserve();

            if (!preservationSucceeded)
            {
                Debug.LogError(
                    "[ExpeditionExitService] Secure inventory " +
                    "preservation failed.");

                return false;
            }

            return TrySavePersistentState(
                "secure Inventory preservation");
        }

        private bool TrySavePersistentState(string operation)
        {
            bool saveSucceeded =
                persistentStateSaver.SavePersistentState();

            if (!saveSucceeded)
            {
                Debug.LogError(
                    "[ExpeditionExitService] Persistent state save " +
                    "failed after " + operation + ".");
            }

            return saveSucceeded;
        }

        private void ReturnToDestination(
            ExpeditionExitDestination destination)
        {
            try
            {
                expeditionDirector.AbortExpedition();
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "[ExpeditionExitService] Failed to abort expedition: "
                    + exception);
            }

            try
            {
                gameFlow.ResetFlow();
                gameFlow.SetEnvironment(GameFlowEnvironment.Sanctuary);

                GameScene targetScene =
                    destination == ExpeditionExitDestination.MainMenu
                        ? GameScene.MainMenu
                        : GameScene.Sanctuary;

                sceneTransitionService.LoadScene(targetScene);
                gameFlow.Initialize();
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "[ExpeditionExitService] Failed to return to destination: "
                    + exception);
            }
        }

        public bool PrepareExit(
            ExpeditionExitReason reason,
            RuntimeExpeditionConfig expeditionConfig)
        {
            if (exitRequested)
            {
                Debug.LogWarning(
                    "[ExpeditionExitService] An exit is already in progress.");
                return false;
            }

            exitRequested = true;

            try
            {
                bool success;

                if (reason == ExpeditionExitReason.Completed)
                {
                    success = HandleCompletedExit(expeditionConfig);
                }
                else
                {
                    success = HandleNonCompletedExit();
                }

                if (!success)
                    exitRequested = false;

                return success;
            }
            catch (Exception exception)
            {
                exitRequested = false;

                Debug.LogError(
                    "[ExpeditionExitService] Exit preparation failed: "
                    + exception);

                return false;
            }
        }

        public void CompleteExit(ExpeditionExitDestination destination)
        {
            if (!exitRequested)
            {
                Debug.LogWarning(
                    "[ExpeditionExitService] Cannot complete an exit that " +
                    "has not been prepared.");
                return;
            }

            ReturnToDestination(destination);
        }

        public void ResetForNewExpedition()
        {
            exitRequested = false;
        }
    }
}