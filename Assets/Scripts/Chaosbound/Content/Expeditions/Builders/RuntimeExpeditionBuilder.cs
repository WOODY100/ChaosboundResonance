using Chaosbound.Content.Expeditions.Definitions.ExpeditionEvents;
using Chaosbound.Content.Expeditions.Definitions.Minimap;
using Chaosbound.Content.Expeditions.Definitions.Rewards;
using Chaosbound.Content.Expeditions.Definitions.Scene;
using Chaosbound.Content.Expeditions.Definitions.Spawn;
using Chaosbound.Content.Expeditions.Definitions.Timeline;
using Chaosbound.Content.Expeditions.Definitions.World;
using Chaosbound.Content.Expeditions.Directors.Timeline;
using Chaosbound.Content.Expeditions.Requests;
using Chaosbound.Content.Expeditions.Runtime.Bosses;
using Chaosbound.Content.Expeditions.Runtime.Combat;
using Chaosbound.Content.Expeditions.Runtime.Completion;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Content.Expeditions.Runtime.ExpeditionEvents;
using Chaosbound.Content.Expeditions.Runtime.MiniBosses;
using Chaosbound.Content.Expeditions.Runtime.Minimap;
using Chaosbound.Content.Expeditions.Runtime.Rewards;
using Chaosbound.Content.Expeditions.Runtime.Scene;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Content.Expeditions.Runtime.Timeline;
using Chaosbound.Content.Expeditions.Runtime.World;
using System;

namespace Chaosbound.Content.Expeditions.Runtime.Builders
{
    /// <summary>
    /// Builds runtime configurations from expedition content.
    /// </summary>
    public sealed class RuntimeExpeditionBuilder
    {
        private readonly RuntimeEnemyBuilder runtimeEnemyBuilder;

        private readonly RuntimeCombatBuilder runtimeCombatBuilder;

        private readonly RuntimeBossesBuilder runtimeBossesBuilder;

        private readonly RuntimeMiniBossesBuilder runtimeMiniBossesBuilder;

        private readonly RuntimeCompletionBuilder runtimeCompletionBuilder;

        public RuntimeExpeditionConfig BuildRunConfig(
            ExpeditionRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Build runtime configurations.
            RuntimeSceneConfig scene = BuildScene(request.Definition.Scene);
            RuntimeWorldConfig world = BuildWorld(request.Definition.World);
            RuntimeEnemyConfig enemy = runtimeEnemyBuilder.BuildEnemy(request.Definition.Enemy);
            RuntimeSpawnConfig spawn = BuildSpawn(request.Definition.Spawn);
            RuntimeCombatConfig combat = runtimeCombatBuilder.BuildCombat(request.Definition.Combat);
            RuntimeTimelineConfig timeline = BuildTimeline(request.Definition.Timeline);
            RuntimeExpeditionEventsConfig expeditionEvents = BuildExpeditionEvents(request.Definition.ExpeditionEvents);
            RuntimeMiniBossesConfig miniBosses = runtimeMiniBossesBuilder.BuildMiniBosses(request.Definition.MiniBosses);
            RuntimeBossesConfig bosses = runtimeBossesBuilder.BuildBosses(request.Definition.Bosses);
            RuntimeRewardsConfig rewards = BuildRewards(request.Definition.Rewards);
            RuntimeCompletionConfig completion = runtimeCompletionBuilder.BuildCompletion(request.Definition.Completion);
            RuntimeMinimapConfig minimap = BuildMinimap(request.Definition.Minimap);

            // Assemble runtime.
            RuntimeExpeditionConfig runtimeConfig =
                new RuntimeExpeditionConfig(
                    scene,
                    world,
                    enemy,
                    spawn,
                    combat,
                    timeline,
                    expeditionEvents,
                    miniBosses,
                    bosses,
                    rewards,
                    completion,
                    minimap);

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

        private RuntimeWorldConfig BuildWorld(
            WorldDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return new RuntimeWorldConfig(
                definition.Bounds,
                definition.Theme);
        }

        private RuntimeSpawnConfig BuildSpawn(
            SpawnDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return new RuntimeSpawnConfig(
                definition.Placement,
                definition.Activation,
                definition.SpawnConstraints);
        }

        public RuntimeExpeditionBuilder(
            RuntimeEnemyBuilder runtimeEnemyBuilder,
            RuntimeCombatBuilder runtimeCombatBuilder,
            RuntimeBossesBuilder runtimeBossesBuilder,
            RuntimeMiniBossesBuilder runtimeMiniBossesBuilder,
            RuntimeCompletionBuilder runtimeCompletionBuilder)
        {
            this.runtimeEnemyBuilder =
                runtimeEnemyBuilder
                ?? throw new ArgumentNullException(
                    nameof(runtimeEnemyBuilder));

            this.runtimeCombatBuilder =
                runtimeCombatBuilder
                ?? throw new ArgumentNullException(
                    nameof(runtimeCombatBuilder));

            this.runtimeBossesBuilder =
                runtimeBossesBuilder
                ?? throw new ArgumentNullException(
                    nameof(runtimeBossesBuilder));

            this.runtimeMiniBossesBuilder =
            runtimeMiniBossesBuilder
            ?? throw new ArgumentNullException(
                nameof(runtimeMiniBossesBuilder));

            this.runtimeCompletionBuilder =
            runtimeCompletionBuilder
            ?? throw new ArgumentNullException(
                nameof(runtimeCompletionBuilder));
        }

        private RuntimeTimelineConfig BuildTimeline(
            TimelineContent content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            TimelineAgenda agenda =
                TimelineDirector.Build(content);

            return new RuntimeTimelineConfig(
                agenda);
        }

        private RuntimeExpeditionEventsConfig BuildExpeditionEvents(
            ExpeditionEventsDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return new RuntimeExpeditionEventsConfig(
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

        private RuntimeMinimapConfig BuildMinimap(
            MinimapDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(
                    nameof(definition));

            return new RuntimeMinimapConfig(
                definition.WalkableTexture);
        }

        #endregion
    }
}