using Chaosbound.Runtime.Run.Configs.General;
using Chaosbound.Content.Expeditions.Configs;
using Chaosbound.Runtime.Run.Configs.World;
using System;

namespace Chaosbound.Runtime.Run
{
    /// <summary>
    /// Immutable contract representing a fully constructed expedition.
    /// Every runtime system consumes this configuration.
    /// </summary>
    public sealed class RuntimeRunConfig
    {
        public GeneralConfig General { get; }

        public WorldConfig World { get; }

        public PopulationConfig Population { get; }

        public PressureConfig Pressure { get; }

        public TimelineConfig Timeline { get; }

        public EventConfig Events { get; }

        public BossConfig Boss { get; }

        public RewardConfig Rewards { get; }

        public PlayerRunConfig Player { get; }

        public RandomConfig Random { get; }

        public RuntimeRunConfig(
            GeneralConfig general,
            WorldConfig world,
            PopulationConfig population,
            PressureConfig pressure,
            TimelineConfig timeline,
            EventConfig events,
            BossConfig boss,
            RewardConfig rewards,
            PlayerRunConfig player,
            RandomConfig random)
        {
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