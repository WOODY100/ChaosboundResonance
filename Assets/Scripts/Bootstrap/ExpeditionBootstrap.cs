using Chaosbound.Content.Expeditions.Assets;
using Chaosbound.Content.Expeditions.Builders.Expedition;
using Chaosbound.Content.Expeditions.Definitions;
using Chaosbound.Content.Expeditions.Enums;
using Chaosbound.Content.Expeditions.Requests;
using Chaosbound.Content.Expeditions.Runtime.Builders;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.Composition;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Shared.Content.Registry;
using Chaosbound.Shared.Content.Resolution;
using System;
using UnityEngine;

namespace Chaosbound.Runtime.Bootstrap
{
    /// <summary>
    /// Builds the runtime configuration for an expedition
    /// and stores it in the active RunSession.
    /// </summary>
    public sealed class ExpeditionBootstrap : MonoBehaviour
    {
        [Header("Expedition")]

        [SerializeField]
        private ExpeditionAsset expeditionAsset;

        private readonly ExpeditionBuilder expeditionBuilder =
            new ExpeditionBuilder();

        public void StartExpedition()
        {
            if (expeditionAsset == null)
            {
                Debug.LogError("Missing ExpeditionAsset.");
                return;
            }

            ExpeditionDefinition expeditionDefinition =
                expeditionBuilder.Build(expeditionAsset);

            BootstrapContext context = BootstrapContext.Current;

            if (context == null)
            {
                Debug.LogError("BootstrapContext is not available.");
                return;
            }

            RunSession runSession = context.RunSession;

            if (runSession == null)
            {
                Debug.LogError("Missing RunSession.");
                return;
            }

            SceneTransitionService sceneTransitionService =
                context.SceneTransitionService;

            if (sceneTransitionService == null)
            {
                Debug.LogError("Missing SceneTransitionService.");
                return;
            }

            RuntimeExpeditionBuilder runtimeBuilder =
                CreateRuntimeBuilder(expeditionDefinition);

            ExpeditionRequest request =
                new ExpeditionRequest(
                    expeditionDefinition,
                    // TODO:
                    // Replace with the player's selected difficulty.
                    DifficultyTier.Normal,
                    Environment.TickCount);

            RuntimeExpeditionConfig runtime =
                runtimeBuilder.BuildRunConfig(request);

            runSession.SetRun(runtime);

            sceneTransitionService.LoadScene(GameScene.Expedition);

            Debug.Log(
                $"Run Ready | Difficulty: {request.SelectedDifficulty} | Seed: {request.Seed}");

            Debug.Log(
                $"RunSession Active: {runSession.HasRun}");
        }

        /// <summary>
        /// Composes the runtime dependency graph required to build
        /// an expedition runtime configuration.
        /// </summary>
        private RuntimeExpeditionBuilder CreateRuntimeBuilder(
            ExpeditionDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            // Build the runtime content registry.
            IContentRegistry registry =
                new ContentRegistryBuilder()
                    .Build(definition.Enemy.Entries);

            // Create the content resolver.
            IContentResolver resolver =
                new UnityContentResolver(registry);

            // Create domain runtime builders.
            RuntimeEnemyBuilder enemyBuilder =
                new RuntimeEnemyBuilder(resolver);

            RuntimeCombatBuilder combatBuilder =
                new RuntimeCombatBuilder();

            // Create the expedition runtime builder.
            return new RuntimeExpeditionBuilder(
                enemyBuilder,
                combatBuilder);
        }
    }
}