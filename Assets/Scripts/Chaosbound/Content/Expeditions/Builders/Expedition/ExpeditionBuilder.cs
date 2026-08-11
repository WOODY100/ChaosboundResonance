using Chaosbound.Content.Expeditions.Assets;
using Chaosbound.Content.Expeditions.Authoring;
using Chaosbound.Content.Expeditions.Builders.Bosses;
using Chaosbound.Content.Expeditions.Builders.ExpeditionEvents;
using Chaosbound.Content.Expeditions.Builders.General;
using Chaosbound.Content.Expeditions.Builders.Identity;
using Chaosbound.Content.Expeditions.Builders.MiniBosses;
using Chaosbound.Content.Expeditions.Builders.Enemy;
using Chaosbound.Content.Expeditions.Builders.Spawn;
using Chaosbound.Content.Expeditions.Builders.Combat;
using Chaosbound.Content.Expeditions.Builders.Presentation;
using Chaosbound.Content.Expeditions.Builders.Rewards;
using Chaosbound.Content.Expeditions.Builders.Scene;
using Chaosbound.Content.Expeditions.Builders.World;
using Chaosbound.Content.Expeditions.Definitions;
using Chaosbound.Content.Expeditions.Definitions.Bosses;
using Chaosbound.Content.Expeditions.Definitions.ExpeditionEvents;
using Chaosbound.Content.Expeditions.Definitions.General;
using Chaosbound.Content.Expeditions.Definitions.Identity;
using Chaosbound.Content.Expeditions.Definitions.MiniBosses;
using Chaosbound.Content.Expeditions.Definitions.Enemy;
using Chaosbound.Content.Expeditions.Definitions.Spawn;
using Chaosbound.Content.Expeditions.Definitions.Combat;
using Chaosbound.Content.Expeditions.Definitions.Presentation;
using Chaosbound.Content.Expeditions.Definitions.Rewards;
using Chaosbound.Content.Expeditions.Definitions.Scene;
using Chaosbound.Content.Expeditions.Definitions.World;
using System;

namespace Chaosbound.Content.Expeditions.Builders.Expedition
{
    /// <summary>
    /// Builds the immutable expedition definition from expedition content.
    /// </summary>
    public sealed class ExpeditionBuilder
    {
        public ExpeditionDefinition Build(
            ExpeditionAsset asset)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset));

            ExpeditionAuthoring authoring = asset.Expedition;

            if (authoring == null)
                throw new InvalidOperationException(
                    "ExpeditionAsset does not contain ExpeditionAuthoring.");

            SceneDefinition scene =
                SceneBuilder.Build(authoring.Scene);

            IdentityDefinition identity =
                IdentityBuilder.Build(authoring.Identity);

            PresentationDefinition presentation =
                PresentationBuilder.Build(authoring.Presentation);

            GeneralDefinition general =
                GeneralBuilder.Build(authoring.General);

            WorldDefinition world =
                WorldBuilder.Build(authoring.World);

            EnemyDefinition enemy =
                EnemyBuilder.Build(authoring.Enemy);

            SpawnDefinition spawn =
                SpawnBuilder.Build(authoring.Spawn);

            CombatDefinition combat =
                CombatBuilder.Build(authoring.Combat);

            ExpeditionEventsDefinition expeditionEvents =
                ExpeditionEventsBuilder.Build(authoring.ExpeditionEvents);

            MiniBossesDefinition miniBosses =
                MiniBossesBuilder.Build(authoring.MiniBosses);

            BossesDefinition bosses =
                BossesBuilder.Build(authoring.Bosses);

            RewardsDefinition rewards =
                RewardsBuilder.Build(authoring.Rewards);

            return new ExpeditionDefinition(
                scene,
                identity,
                presentation,
                general,
                world,
                enemy,
                spawn,
                combat,
                expeditionEvents,
                miniBosses,
                bosses,
                rewards);
        }
    }
}