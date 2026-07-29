using Chaosbound.Content.Expeditions.Definitions.Enemy;
using Chaosbound.Content.Expeditions.Definitions.Rewards;
using Chaosbound.Content.Expeditions.Definitions.ExpeditionEvents;
using Chaosbound.Content.Expeditions.Definitions.Bosses;
using Chaosbound.Content.Expeditions.Definitions.General;
using Chaosbound.Content.Expeditions.Definitions.MiniBosses;
using Chaosbound.Content.Expeditions.Definitions.World;
using Chaosbound.Content.Expeditions.Runtime.Bosses;
using Chaosbound.Content.Expeditions.Runtime.ExpeditionEvents;
using Chaosbound.Content.Expeditions.Runtime.General;
using Chaosbound.Content.Expeditions.Runtime.MiniBosses;
using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Content.Expeditions.Runtime.Rewards;
using Chaosbound.Content.Expeditions.Runtime.World;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Content.Expeditions.Requests;
using Chaosbound.Content.Expeditions.Definitions.Scene;
using Chaosbound.Content.Expeditions.Runtime.Scene;
using System;

namespace Chaosbound.Content.Expeditions.Runtime.Builders
{
    /// <summary>
    /// Builds runtime configurations from expedition content.
    /// </summary>
    public sealed class RuntimeExpeditionBuilder
    {
        private readonly RuntimeEnemyBuilder runtimeEnemyBuilder;

        public RuntimeExpeditionBuilder(
            RuntimeEnemyBuilder runtimeEnemyBuilder)
        {
            this.runtimeEnemyBuilder = runtimeEnemyBuilder
                ?? throw new ArgumentNullException(nameof(runtimeEnemyBuilder));
        }

        public RuntimeExpeditionConfig BuildRunConfig(
            ExpeditionRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Build runtime configurations.
            RuntimeSceneConfig scene = BuildScene(request.Definition.Scene);
            RuntimeGeneralConfig general = BuildGeneral(request.Definition.General);
            RuntimeWorldConfig world = BuildWorld(request.Definition.World);
            RuntimeEnemyConfig enemy = runtimeEnemyBuilder.Build(request.Definition.Enemy);
            RuntimeExpeditionEventsConfig expeditionEvents = BuildExpeditionEvents(request.Definition.ExpeditionEvents);
            RuntimeMiniBossesConfig miniBosses = BuildMiniBosses(request.Definition.MiniBosses);
            RuntimeBossesConfig bosses = BuildBosses(request.Definition.Bosses);
            RuntimeRewardsConfig rewards = BuildRewards(request.Definition.Rewards);

            // Assemble runtime.
            RuntimeExpeditionConfig runtimeConfig =
                new RuntimeExpeditionConfig(
                    scene,
                    general,
                    world,
                    enemy,
                    expeditionEvents,
                    miniBosses,
                    bosses,
                    rewards);

            return runtimeConfig;
        }

        #region Runtime Builders

        private RuntimeSceneConfig BuildScene(
            SceneDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return new RuntimeSceneConfig(
                definition.SceneName);
        }

        private RuntimeGeneralConfig BuildGeneral(
            GeneralDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return new RuntimeGeneralConfig(
                definition.CompletionCondition,
                definition.BaseDifficulty);
        }

        private RuntimeWorldConfig BuildWorld(
            WorldDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return new RuntimeWorldConfig(
                definition.Bounds,
                definition.Theme);
        }

        private RuntimeExpeditionEventsConfig BuildExpeditionEvents(
            ExpeditionEventsDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return new RuntimeExpeditionEventsConfig(
                definition.Content);
        }
        private RuntimeMiniBossesConfig BuildMiniBosses(
            MiniBossesDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));
            return new RuntimeMiniBossesConfig(
                definition.Content);
        }

        private RuntimeBossesConfig BuildBosses(
            BossesDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));
            return new RuntimeBossesConfig(
                definition.Content);
        }

        private RuntimeRewardsConfig BuildRewards(
            RewardsDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));
            return new RuntimeRewardsConfig(
                definition.Content);
        }

        #endregion
    }
}