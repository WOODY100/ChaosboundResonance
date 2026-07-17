using Chaosbound.Content.Expeditions.Definitions;
using Chaosbound.Content.Expeditions.Definitions.Population;
using System;

namespace Chaosbound.Content.Expeditions
{
    /// <summary>
    /// Represents a fully designed expedition.
    /// </summary>
    public sealed class ExpeditionDefinition
    {
        public ExpeditionIdentity Identity { get; }

        public GeneralDefinition General { get; }

        public WorldDefinition World { get; }

        public PopulationDefinition Population { get; }

        public PressureDefinition Pressure { get; }

        public TimelineDefinition Timeline { get; }

        public ExpeditionEventDefinition Events { get; }

        public BossDefinition Boss { get; }

        public RewardDefinition Rewards { get; }

        public PlayerDefinition Player { get; }

        public RandomDefinition Random { get; }

        public ExpeditionDefinition(
            ExpeditionIdentity identity,
            GeneralDefinition general,
            WorldDefinition world,
            PopulationDefinition population,
            PressureDefinition pressure,
            TimelineDefinition timeline,
            ExpeditionEventDefinition events,
            BossDefinition boss,
            RewardDefinition rewards,
            PlayerDefinition player,
            RandomDefinition random)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            General = general ?? throw new ArgumentNullException(nameof(general));
            World = world ?? throw new ArgumentNullException(nameof(world));
            Population = population ?? throw new ArgumentNullException(nameof(population));
            Pressure = pressure ?? throw new ArgumentNullException(nameof(pressure));
            Timeline = timeline ?? throw new ArgumentNullException(nameof(timeline));
            Events = events ?? throw new ArgumentNullException(nameof(events));
            Boss = boss ?? throw new ArgumentNullException(nameof(boss));
            Rewards = rewards ?? throw new ArgumentNullException(nameof(rewards));
            Player = player ?? throw new ArgumentNullException(nameof(player));
            Random = random ?? throw new ArgumentNullException(nameof(random));
        }
    }
}