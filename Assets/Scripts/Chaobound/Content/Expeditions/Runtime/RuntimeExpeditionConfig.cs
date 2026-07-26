using Chaosbound.Content.Expeditions.Runtime.Bosses;
using Chaosbound.Content.Expeditions.Runtime.ExpeditionEvents;
using Chaosbound.Content.Expeditions.Runtime.General;
using Chaosbound.Content.Expeditions.Runtime.MiniBosses;
using Chaosbound.Content.Expeditions.Runtime.Population;
using Chaosbound.Content.Expeditions.Runtime.Rewards;
using Chaosbound.Content.Expeditions.Runtime.World;
using Chaosbound.Content.Expeditions.Runtime.Scene;
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

        public RuntimeGeneralConfig General { get; }

        public RuntimeWorldConfig World { get; }

        public RuntimePopulationConfig Population { get; }

        public RuntimeExpeditionEventsConfig ExpeditionEvents { get; }
        
        public RuntimeMiniBossesConfig MiniBosses { get; }

        public RuntimeBossesConfig Bosses { get; }

        public RuntimeRewardsConfig Rewards { get; }

        public RuntimeExpeditionConfig(
            RuntimeSceneConfig scene,
            RuntimeGeneralConfig general,
            RuntimeWorldConfig world,
            RuntimePopulationConfig population,
            RuntimeExpeditionEventsConfig expeditionEvents,
            RuntimeMiniBossesConfig miniBosses,
            RuntimeBossesConfig bosses,
            RuntimeRewardsConfig rewards)
        {
            Scene = scene ?? throw new ArgumentNullException(nameof(scene));
            General = general ?? throw new ArgumentNullException(nameof(general));
            World = world ?? throw new ArgumentNullException(nameof(world));
            Population = population ?? throw new ArgumentNullException(nameof(population));
            ExpeditionEvents = expeditionEvents ?? throw new ArgumentNullException(nameof(expeditionEvents));
            MiniBosses = miniBosses ?? throw new ArgumentNullException(nameof(miniBosses));
            Bosses = bosses ?? throw new ArgumentNullException(nameof(bosses));
            Rewards = rewards ?? throw new ArgumentNullException(nameof(rewards));
        }
    }
}