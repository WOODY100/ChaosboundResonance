using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Content.Expeditions.Assets;
using Chaosbound.Content.Expeditions.Builders.Expedition;
using Chaosbound.Content.Expeditions.Definitions;
using Chaosbound.Content.Expeditions.Enums;
using Chaosbound.Content.Expeditions.Requests;
using Chaosbound.Content.Expeditions.Runtime.Builders;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Core.Composition;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Shared.Content.Entries;
using Chaosbound.Shared.Content.Registry;
using Chaosbound.Shared.Content.Resolution;
using System;
using System.Collections.Generic;
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

        private readonly ExpeditionBuilder expeditionBuilder =
            new ExpeditionBuilder();

        public void StartExpedition(ExpeditionAsset selectedExpedition)
        {
            if (selectedExpedition == null)
            {
                Debug.LogError(
                    "[ExpeditionBootstrap] No expedition was selected.",
                    this);

                return;
            }

            ExpeditionDefinition expeditionDefinition;

            try
            {
                expeditionDefinition = expeditionBuilder.Build(selectedExpedition);
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[ExpeditionBootstrap] Failed to build expedition '{selectedExpedition.name}'.",
                    selectedExpedition);

                Debug.LogException(exception, selectedExpedition);
                return;
            }

            BootstrapContext context = BootstrapContext.Current;

            if (context == null)
            {
                Debug.LogError("BootstrapContext is not available.", this);
                return;
            }

            RunSession runSession = context.RunSession;

            if (runSession == null)
            {
                Debug.LogError("Missing RunSession.", this);
                return;
            }

            SceneTransitionService sceneTransitionService =
                context.SceneTransitionService;

            if (sceneTransitionService == null)
            {
                Debug.LogError("Missing SceneTransitionService.", this);
                return;
            }

            RuntimeExpeditionBuilder runtimeBuilder =
                CreateRuntimeBuilder(expeditionDefinition);

            ExpeditionRequest request = new ExpeditionRequest(
                expeditionDefinition,
                DifficultyTier.Normal,
                Environment.TickCount);

            RuntimeExpeditionConfig runtime =
                runtimeBuilder.BuildRunConfig(request);

            runSession.SetRun(runtime);

            sceneTransitionService.LoadScene(
                GameScene.Expedition,
                waitForSceneReady: true);
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
            List<ContentEntry> contentEntries =
                new();

            contentEntries.AddRange(
                definition.Enemy.Entries);

            contentEntries.AddRange(
                definition.MiniBosses.Entries);

            contentEntries.AddRange(
                definition.Bosses.Entries);

            IContentRegistry registry =
                new ContentRegistryBuilder()
                    .Build(contentEntries);

            // Create the content resolver.
            IContentResolver resolver =
                new UnityContentResolver(registry);

            // Create domain runtime builders.
            RuntimeEnemyBuilder enemyBuilder =
                new RuntimeEnemyBuilder(resolver);

            RuntimeCombatBuilder combatBuilder =
                new RuntimeCombatBuilder();

            RuntimeBossesBuilder bossesBuilder =
                new RuntimeBossesBuilder(resolver);

            RuntimeMiniBossesBuilder miniBossesBuilder =
                new RuntimeMiniBossesBuilder(resolver);

            RuntimeCompletionBuilder completionBuilder =
                new RuntimeCompletionBuilder();

            // Create the expedition runtime builder.
            return new RuntimeExpeditionBuilder(
                enemyBuilder,
                combatBuilder,
                bossesBuilder,
                miniBossesBuilder,
                completionBuilder);
        }
    }
}