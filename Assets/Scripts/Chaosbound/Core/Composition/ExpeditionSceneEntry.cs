using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.GameFlow;
using Chaosbound.Gameplay.ExpeditionRuntime.Director;
using Chaosbound.Gameplay.ExpeditionRuntime.Exit;
using Chaosbound.Gameplay.ExpeditionRuntime.Result;
using System;
using UnityEngine;

namespace Chaosbound.Core.Composition
{
    public sealed class ExpeditionSceneEntry : MonoBehaviour
    {
        [Header("Scene Context")]
        [SerializeField]
        private ExpeditionSceneContext sceneContext;

        private ExpeditionComposition composition;

        private BootstrapContext bootstrapContext;

        private ExpeditionResultBuilder resultBuilder;

        private ExpeditionExitReason currentExitReason;

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            composition?.Tick();
        }

        private void OnDestroy()
        {
            UnsubscribeFromGameFlow();
            UnsubscribeFromResultPanel();
            UnsubscribeFromExpeditionExit();

            composition?.Cleanup();
        }

        private void Initialize()
        {
            Validate();

            bootstrapContext =
                BootstrapContext.Current;

            RunSession runSession =
                bootstrapContext.RunSession;

            RuntimeExpeditionConfig runtimeConfig =
                runSession.CurrentRun;

            composition =
                CreateComposition(
                    bootstrapContext,
                    runtimeConfig);

            composition.Initialize();

            resultBuilder =
                new ExpeditionResultBuilder();

            SubscribeToGameFlow();
            SubscribeToResultPanel();
            SubscribeToExpeditionExit();
        }

        private ExpeditionComposition CreateComposition(
            BootstrapContext bootstrap,
            RuntimeExpeditionConfig runtimeConfig)
        {
            return new ExpeditionComposition(
                bootstrap,
                runtimeConfig,
                sceneContext);
        }

        private void SubscribeToExpeditionExit()
        {
            if (composition == null)
                throw new InvalidOperationException(
                    "ExpeditionComposition is required before subscribing to expedition exit.");

            ExpeditionDirector director =
                bootstrapContext.RunManager.ExpeditionDirector;

            if (director == null)
                throw new InvalidOperationException(
                    "ExpeditionDirector is required before subscribing to expedition exit.");

            director.ExpeditionExitAccepted +=
                HandleExpeditionExitAccepted;
        }

        private void UnsubscribeFromExpeditionExit()
        {
            if (bootstrapContext == null ||
                bootstrapContext.RunManager == null)
                return;

            ExpeditionDirector director =
                bootstrapContext.RunManager.ExpeditionDirector;

            if (director == null)
                return;

            director.ExpeditionExitAccepted -=
                HandleExpeditionExitAccepted;
        }

        //==========================================================
        // Result
        //==========================================================

        private void SubscribeToGameFlow()
        {
            if (bootstrapContext == null)
            {
                throw new InvalidOperationException(
                    "BootstrapContext is required before subscribing to GameFlow.");
            }

            if (bootstrapContext.GameFlow == null)
            {
                throw new InvalidOperationException(
                    "GameFlow is required before subscribing to Result flow.");
            }

            bootstrapContext.GameFlow.OnContextChanged +=
                HandleGameFlowContextChanged;
        }

        private void UnsubscribeFromGameFlow()
        {
            if (bootstrapContext == null ||
                bootstrapContext.GameFlow == null)
            {
                return;
            }

            bootstrapContext.GameFlow.OnContextChanged -=
                HandleGameFlowContextChanged;
        }

        private void SubscribeToResultPanel()
        {
            if (sceneContext.ExpeditionResultPanel == null)
            {
                throw new InvalidOperationException(
                    "ExpeditionResultPanel is required before subscribing to Result actions.");
            }

            sceneContext.ExpeditionResultPanel.ReturnToSanctuaryRequested +=
                HandleReturnToSanctuaryRequested;
        }

        private void UnsubscribeFromResultPanel()
        {
            if (sceneContext == null ||
                sceneContext.ExpeditionResultPanel == null)
            {
                return;
            }

            sceneContext.ExpeditionResultPanel.ReturnToSanctuaryRequested -=
                HandleReturnToSanctuaryRequested;
        }

        private void HandleGameFlowContextChanged(
            GameFlowContext previous,
            GameFlowContext current)
        {
            if (current != GameFlowContext.GameOver)
                return;

            ShowFailedResult();
        }

        private void HandleExpeditionExitAccepted()
        {
            ShowCompletedResult();
        }

        private void ShowCompletedResult()
        {
            if (resultBuilder == null)
            {
                Debug.LogError(
                    "ExpeditionResultBuilder is not initialized.",
                    this);
                return;
            }

            RunManager runManager =
                bootstrapContext.RunManager;

            if (runManager == null)
            {
                Debug.LogError(
                    "RunManager is not available.",
                    this);
                return;
            }

            if (sceneContext.PlayerExperienceSystem == null)
            {
                Debug.LogError(
                    "PlayerExperienceSystem is not available.",
                    this);
                return;
            }

            if (sceneContext.PlayerSkillLoadout == null)
            {
                Debug.LogError(
                    "PlayerSkillLoadout is not available.",
                    this);
                return;
            }

            if (sceneContext.ExpeditionResultPanel == null)
            {
                Debug.LogError(
                    "ExpeditionResultPanel is not available.",
                    this);
                return;
            }

            ExpeditionResultData resultData =
                resultBuilder.Build(
                    ExpeditionResultStatus.Completed,
                    runManager.ExpeditionRuntimeState,
                    sceneContext.PlayerExperienceSystem,
                    sceneContext.PlayerSkillLoadout);

            currentExitReason =
                ExpeditionExitReason.Completed;

            sceneContext.ExpeditionResultPanel.Show(resultData);

            bootstrapContext.GameFlow.Replace(
                GameFlowContext.Result);
        }

        private void ShowFailedResult()
        {
            if (resultBuilder == null)
            {
                Debug.LogError(
                    "ExpeditionResultBuilder is not initialized.",
                    this);

                return;
            }

            RunManager runManager =
                bootstrapContext.RunManager;

            if (runManager == null)
            {
                Debug.LogError(
                    "RunManager is not available.",
                    this);

                return;
            }

            if (sceneContext.PlayerExperienceSystem == null)
            {
                Debug.LogError(
                    "PlayerExperienceSystem is not available.",
                    this);

                return;
            }

            if (sceneContext.PlayerSkillLoadout == null)
            {
                Debug.LogError(
                    "PlayerSkillLoadout is not available.",
                    this);

                return;
            }

            if (sceneContext.ExpeditionResultPanel == null)
            {
                Debug.LogError(
                    "ExpeditionResultPanel is not available.",
                    this);

                return;
            }

            ExpeditionResultData resultData =
                resultBuilder.Build(
                    ExpeditionResultStatus.Failed,
                    runManager.ExpeditionRuntimeState,
                    sceneContext.PlayerExperienceSystem,
                    sceneContext.PlayerSkillLoadout);

            currentExitReason =
                ExpeditionExitReason.Death;

            sceneContext.ExpeditionResultPanel.Show(
                resultData);
        }

        private void HandleReturnToSanctuaryRequested()
        {
            if (bootstrapContext == null)
            {
                Debug.LogError(
                    "BootstrapContext is not available.",
                    this);

                return;
            }

            RunManager runManager =
                bootstrapContext.RunManager;

            if (runManager == null)
            {
                Debug.LogError(
                    "RunManager is not available.",
                    this);

                return;
            }

            if (runManager.ExpeditionExitService == null)
            {
                Debug.LogError(
                    "ExpeditionExitService is not available.",
                    this);

                return;
            }

            runManager.ExpeditionExitService.Exit(
                currentExitReason);
        }

        //==========================================================
        // Validation
        //==========================================================

        private void Validate()
        {
            BootstrapContext bootstrap =
                BootstrapContext.Current;

            if (bootstrap == null)
            {
                throw new InvalidOperationException(
                    "BootstrapContext was not found.");
            }

            if (sceneContext == null)
            {
                throw new InvalidOperationException(
                    "ExpeditionSceneContext reference is missing.");
            }

            RunSession runSession =
                bootstrap.RunSession;

            if (runSession == null)
            {
                throw new InvalidOperationException(
                    "RunSession reference is missing.");
            }

            if (!runSession.HasRun)
            {
                throw new InvalidOperationException(
                    "There is no active RunSession.");
            }
        }
    }
}