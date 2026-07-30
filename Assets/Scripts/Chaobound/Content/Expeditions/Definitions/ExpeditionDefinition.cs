using Chaosbound.Content.Expeditions.Definitions.Bosses;
using Chaosbound.Content.Expeditions.Definitions.Enemy;
using Chaosbound.Content.Expeditions.Definitions.ExpeditionEvents;
using Chaosbound.Content.Expeditions.Definitions.General;
using Chaosbound.Content.Expeditions.Definitions.Identity;
using Chaosbound.Content.Expeditions.Definitions.MiniBosses;
using Chaosbound.Content.Expeditions.Definitions.Presentation;
using Chaosbound.Content.Expeditions.Definitions.Rewards;
using Chaosbound.Content.Expeditions.Definitions.Scene;
using Chaosbound.Content.Expeditions.Definitions.Spawn;
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
        /// Gets the general expedition configuration.
        /// </summary>
        public GeneralDefinition General { get; }

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

        public ExpeditionDefinition(
            SceneDefinition scene,
            IdentityDefinition identity,
            PresentationDefinition presentation,
            GeneralDefinition general,
            WorldDefinition world,
            EnemyDefinition enemy,
            SpawnDefinition spawn,
            ExpeditionEventsDefinition expeditionEvents,
            MiniBossesDefinition miniBosses,
            BossesDefinition bosses,
            RewardsDefinition rewards)
        {
            if (scene == null)
                throw new ArgumentNullException(nameof(scene));

            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            if (presentation == null)
                throw new ArgumentNullException(nameof(presentation));

            if (general == null)
                throw new ArgumentNullException(nameof(general));

            if (world == null)
                throw new ArgumentNullException(nameof(world));

            if (enemy == null)
                throw new ArgumentNullException(nameof(enemy));

            if (spawn == null)
                throw new ArgumentNullException(nameof(spawn));

            if (expeditionEvents == null)
                throw new ArgumentNullException(nameof(expeditionEvents));

            if (miniBosses == null)
                throw new ArgumentNullException(nameof(miniBosses));

            if (bosses == null) 
                throw new ArgumentNullException(nameof(bosses));

            if (rewards == null) 
                throw new ArgumentNullException(nameof(rewards));

            Scene = scene;
            Identity = identity;
            Presentation = presentation;
            General = general;
            World = world;
            Enemy = enemy;
            Spawn = spawn;
            ExpeditionEvents = expeditionEvents;
            MiniBosses = miniBosses;
            Bosses = bosses;
            Rewards = rewards;
        }
    }
}