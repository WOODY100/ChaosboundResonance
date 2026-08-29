using Chaosbound.Content.Expeditions.Definitions.Bosses;
using Chaosbound.Content.Expeditions.Definitions.Combat;
using Chaosbound.Content.Expeditions.Definitions.Completion;
using Chaosbound.Content.Expeditions.Definitions.Enemy;
using Chaosbound.Content.Expeditions.Definitions.ExpeditionEvents;
using Chaosbound.Content.Expeditions.Definitions.Identity;
using Chaosbound.Content.Expeditions.Definitions.MiniBosses;
using Chaosbound.Content.Expeditions.Definitions.Minimap;
using Chaosbound.Content.Expeditions.Definitions.Presentation;
using Chaosbound.Content.Expeditions.Definitions.Rewards;
using Chaosbound.Content.Expeditions.Definitions.Scene;
using Chaosbound.Content.Expeditions.Definitions.Spawn;
using Chaosbound.Content.Expeditions.Definitions.Timeline;
using Chaosbound.Content.Expeditions.Definitions.World;
using System;

namespace Chaosbound.Content.Expeditions.Definitions
{
    /// <summary>
    /// Represents the complete immutable definition of an expedition.
    /// </summary>
    public sealed class ExpeditionDefinition
    {
        /// <summary>
        /// Gets the scene configuration.
        /// </summary>
        public SceneDefinition Scene { get; }

        /// <summary>
        /// Gets the identity configuration.
        /// </summary>
        public IdentityDefinition Identity { get; }

        /// <summary>
        /// Gets the presentation configuration.
        /// </summary>
        public PresentationDefinition Presentation { get; }

        /// <summary>
        /// Gets the world generation configuration.
        /// </summary>
        public WorldDefinition World { get; }

        /// <summary>
        /// Gets the enemy configuration.
        /// </summary>
        public EnemyDefinition Enemy { get; }

        /// <summary>
        /// Gets the spawn configuration.
        /// </summary>
        public SpawnDefinition Spawn { get; }

        /// <summary>
        /// Gets the combat configuration.
        /// </summary>
        public CombatDefinition Combat { get; }

        /// <summary>
        /// Gets the timeline configuration.
        /// </summary>
        public TimelineContent Timeline { get; }

        /// <summary>
        /// Gets the expedition events configuration.
        /// </summary>
        public ExpeditionEventsDefinition ExpeditionEvents { get; }

        /// <summary>
        /// Gets the mini Bosses configuration.
        /// </summary>
        public MiniBossesDefinition MiniBosses { get; }

        /// <summary>
        /// Gets the bosses configuration.
        /// </summary>
        public BossesDefinition Bosses { get; }

        /// <summary>
        /// Gets the rewards configuration.
        /// </summary>
        public RewardsDefinition Rewards { get; }

        /// <summary>
        /// Gets the expedition completion configuration.
        /// </summary>
        public CompletionDefinition Completion { get; }

        /// <summary>
        /// Gets the minimap configuration.
        /// </summary>
        public MinimapDefinition Minimap { get; }

        public ExpeditionDefinition(
            SceneDefinition scene,
            IdentityDefinition identity,
            PresentationDefinition presentation,
            WorldDefinition world,
            EnemyDefinition enemy,
            SpawnDefinition spawn,
            CombatDefinition combat,
            TimelineContent timeline,
            ExpeditionEventsDefinition expeditionEvents,
            MiniBossesDefinition miniBosses,
            BossesDefinition bosses,
            RewardsDefinition rewards,
            CompletionDefinition completion,
            MinimapDefinition minimap)
        {
            if (scene == null)
                throw new ArgumentNullException(nameof(scene));

            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            if (presentation == null)
                throw new ArgumentNullException(nameof(presentation));

            if (world == null)
                throw new ArgumentNullException(nameof(world));

            if (enemy == null)
                throw new ArgumentNullException(nameof(enemy));

            if (spawn == null)
                throw new ArgumentNullException(nameof(spawn));

            if (combat == null)
                throw new ArgumentNullException(nameof(combat));

            if (timeline == null)
                throw new ArgumentNullException(nameof(timeline));

            if (expeditionEvents == null)
                throw new ArgumentNullException(nameof(expeditionEvents));

            if (miniBosses == null)
                throw new ArgumentNullException(nameof(miniBosses));

            if (bosses == null) 
                throw new ArgumentNullException(nameof(bosses));

            if (rewards == null) 
                throw new ArgumentNullException(nameof(rewards));

            if (completion == null) 
                throw new ArgumentNullException(nameof(completion));

            if (minimap == null)
                throw new ArgumentNullException(nameof(minimap));

            Scene = scene;
            Identity = identity;
            Presentation = presentation;
            World = world;
            Enemy = enemy;
            Spawn = spawn;
            Combat = combat;
            Timeline = timeline;
            ExpeditionEvents = expeditionEvents;
            MiniBosses = miniBosses;
            Bosses = bosses;
            Rewards = rewards;
            Completion = completion;
            Minimap = minimap;
        }
    }
}