using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.GameFlow;
using Chaosbound.Gameplay.ExpeditionRuntime.Director;
using Chaosbound.Gameplay.ExpeditionRuntime.Exit;
using Chaosbound.Gameplay.ExpeditionRuntime.Result;
using System.Collections;
using System;
using UnityEngine;
using TMPro;

namespace Chaosbound.Core.Composition
{
    public sealed class ExpeditionSceneEntry : MonoBehaviour
    {
        [Header("Scene Context")]
        [SerializeField]
        private ExpeditionSceneContext sceneContext;

        [SerializeField, Min(0f)]
        private float resultDisplayTime = 2f;

        [Header("Expedition Intro")]

        [SerializeField]
        private GameObject expeditionStartedMessage;

        [SerializeField, Min(0f)]
        private float expeditionStartedDisplayTime = 1.5f;

        private Coroutine introCoroutine;

        private Coroutine exitCoroutine;
        private bool isExiting;

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
            if (introCoroutine != null)
            {
                StopCoroutine(introCoroutine);
                introCoroutine = null;
            }

            UnsubscribeFromGameFlow();
            UnsubscribeFromResultPanel();
            UnsubscribeFromExpeditionExit();

            composition?.Cleanup();
        }


        private void Initialize()
        {
            Validate();

            bootstrapContext = BootstrapContext.Current;

            bootstrapContext.GameFlow.SetEnvironment(
                GameFlowEnvironment.Expedition);

            // Establish Playing as the root context.
            bootstrapContext.GameFlow.Initialize();

            // Keep simulation and expedition time paused during preparation.
            bootstrapContext.GameFlow.Replace(
                GameFlowContext.Intro);

            RunSession runSession = bootstrapContext.RunSession;
            RuntimeExpeditionConfig runtimeConfig = runSession.CurrentRun;

            composition = CreateComposition(
                bootstrapContext,
                runtimeConfig);

            // StartRun remains inside composition initialization because
            // the minimap requires ExpeditionRuntimeState.
            composition.Initialize();

            resultBuilder = new ExpeditionResultBuilder();

            SubscribeToGameFlow();
            SubscribeToResultPanel();
            SubscribeToExpeditionExit();

            if (expeditionStartedMessage != null)
                expeditionStartedMessage.SetActive(false);

            // Notify the persistent transition service that the scene
            // has completed its required initialization.
            bootstrapContext.SceneTransitionService.MarkSceneReady();

            introCoroutine = StartCoroutine(
                ShowExpeditionIntroRoutine());
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
                    sceneContext.PlayerSkillLoadout,
                    runManager.CurrentRunConfig);

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
                    sceneContext.PlayerSkillLoadout,
                    runManager.CurrentRunConfig);

            currentExitReason =
                ExpeditionExitReason.Death;

            sceneContext.ExpeditionResultPanel.Show(
                resultData);
        }

        private void HandleReturnToSanctuaryRequested()
        {
            if (isExiting)
                return;

            if (bootstrapContext == null)
            {
                Debug.LogError(
                    "BootstrapContext is not available.",
                    this);
                return;
            }

            if (bootstrapContext.SaveFeedbackController == null)
            {
                Debug.LogError(
                    "SaveFeedbackController is not available.",
                    this);
                return;
            }

            RunManager runManager = bootstrapContext.RunManager;

            if (runManager == null ||
                runManager.ExpeditionExitService == null)
            {
                Debug.LogError(
                    "ExpeditionExitService is not available.",
                    this);
                return;
            }

            isExiting = true;

            exitCoroutine = StartCoroutine(
                CompleteExpeditionRoutine(runManager));
        }

        private IEnumerator CompleteExpeditionRoutine(
            RunManager runManager)
        {
            SaveFeedbackController feedback =
                bootstrapContext.SaveFeedbackController;

            if (feedback == null)
            {
                Debug.LogError(
                    "SaveFeedbackController is not available.",
                    this);

                isExiting = false;
                exitCoroutine = null;
                yield break;
            }

            feedback.ShowSaving();

            yield return null;

            bool success =
                runManager.ExpeditionExitService.PrepareExit(
                    currentExitReason,
                    runManager.CurrentRunConfig);

            feedback.ShowResult(success);

            if (!success)
            {
                isExiting = false;
                exitCoroutine = null;
                yield break;
            }

            yield return new WaitForSecondsRealtime(
                resultDisplayTime);

            runManager.ExpeditionExitService.CompleteExit(
                ExpeditionExitDestination.Sanctuary);
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

        private IEnumerator ShowExpeditionIntroRoutine()
        {
            var transitionService =
                bootstrapContext.SceneTransitionService;

            // Wait until the transition service has hidden the loading screen.
            while (transitionService != null &&
                   transitionService.IsTransitioning)
            {
                yield return null;
            }

            if (expeditionStartedMessage != null)
                expeditionStartedMessage.SetActive(true);

            // Use real time because GameFlow is still paused.
            yield return new WaitForSecondsRealtime(
                expeditionStartedDisplayTime);

            if (expeditionStartedMessage != null)
                expeditionStartedMessage.SetActive(false);

            // Resume simulation and expedition time only after the message.
            if (bootstrapContext != null &&
                bootstrapContext.GameFlow != null)
            {
                bootstrapContext.GameFlow.Replace(
                    GameFlowContext.Playing);
            }

            introCoroutine = null;
        }
    }
}