using Chaosbound.Content.Expeditions.Runtime.Bosses;
using Chaosbound.Content.Expeditions.Runtime.Combat;
using Chaosbound.Content.Expeditions.Runtime.Completion;
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

namespace Chaosbound.Content.Expeditions.Runtime.Configs
{
    /// <summary>
    /// Immutable contract representing a fully constructed expedition.
    /// Every runtime system consumes this configuration.
    /// </summary>
    public sealed class RuntimeExpeditionConfig
    {
        public RuntimeSceneConfig Scene { get; }

        public RuntimeWorldConfig World { get; }

        public RuntimeEnemyConfig Enemy { get; }

        public RuntimeSpawnConfig Spawn { get; }

        public RuntimeCombatConfig Combat { get; }

        public RuntimeSkillProgressionConfig SkillProgression { get; }

        public RuntimeTimelineConfig Timeline { get; }

        public RuntimeExpeditionEventsConfig ExpeditionEvents { get; }
        
        public RuntimeMiniBossesConfig MiniBosses { get; }

        public RuntimeBossesConfig Bosses { get; }

        public RuntimeRewardsConfig Rewards { get; }

        public RuntimeCompletionConfig Completion { get; }

        public RuntimeMinimapConfig Minimap { get; }

        public RuntimeExpeditionConfig(
            RuntimeSceneConfig scene,
            RuntimeWorldConfig world,
            RuntimeEnemyConfig enemy,
            RuntimeSpawnConfig spawn,
            RuntimeCombatConfig combat,
            RuntimeSkillProgressionConfig skillProgression,
            RuntimeTimelineConfig timeline,
            RuntimeExpeditionEventsConfig expeditionEvents,
            RuntimeMiniBossesConfig miniBosses,
            RuntimeBossesConfig bosses,
            RuntimeRewardsConfig rewards,
            RuntimeCompletionConfig completion,
            RuntimeMinimapConfig minimap)
        {
            Scene = scene ?? throw new ArgumentNullException(nameof(scene));
            World = world ?? throw new ArgumentNullException(nameof(world));
            Enemy = enemy ?? throw new ArgumentNullException(nameof(enemy));
            Spawn = spawn ?? throw new ArgumentNullException(nameof(spawn));
            Combat = combat ?? throw new ArgumentNullException(nameof(combat));
            SkillProgression = skillProgression ?? throw new ArgumentNullException(nameof(skillProgression));
            Timeline = timeline ?? throw new ArgumentNullException(nameof(timeline));
            ExpeditionEvents = expeditionEvents ?? throw new ArgumentNullException(nameof(expeditionEvents));
            MiniBosses = miniBosses ?? throw new ArgumentNullException(nameof(miniBosses));
            Bosses = bosses ?? throw new ArgumentNullException(nameof(bosses));
            Rewards = rewards ?? throw new ArgumentNullException(nameof(rewards));
            Completion = completion ?? throw new ArgumentNullException(nameof(completion));
            Minimap = minimap ?? throw new ArgumentNullException(nameof(minimap));
        }
    }
}