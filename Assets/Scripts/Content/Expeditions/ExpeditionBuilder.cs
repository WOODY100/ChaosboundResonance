using Chaosbound.Content.Expeditions.Definitions;
using Chaosbound.Content.Expeditions.Definitions.Population;
using Chaosbound.Runtime.Run;
using Chaosbound.Runtime.Run.Configs.General;
using Chaosbound.Content.Expeditions.Configs;
using Chaosbound.Runtime.Run.Configs.World;
using System;

namespace Chaosbound.Content.Expeditions
{
    /// <summary>
    /// Builds runtime configurations from expedition content.
    /// </summary>
    public sealed class ExpeditionBuilder
    {
        public RuntimeRunConfig BuildRunConfig(
            ExpeditionRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Build runtime configurations.
            GeneralConfig general = BuildGeneral(request.Definition.General);
            WorldConfig world = BuildWorld(request.Definition.World);
            PopulationConfig population = BuildPopulation(request.Definition.Population);

            // Assemble runtime.
            RuntimeRunConfig runtime =
                new RuntimeRunConfig(
                    general,
                    world,
                    population,
                    PressureConfig.Empty,
                    TimelineConfig.Empty,
                    EventConfig.Empty,
                    BossConfig.Empty,
                    RewardConfig.Empty,
                    PlayerRunConfig.Empty,
                    RandomConfig.Empty);

            return runtime;
        }

        #region Runtime Builders

        private GeneralConfig BuildGeneral(
            GeneralDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return new GeneralConfig(
                definition.CompletionCondition,
                definition.BaseDifficulty);
        }

        private WorldConfig BuildWorld(
            WorldDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return new WorldConfig(
                definition.Bounds);
        }

        private PopulationConfig BuildPopulation(
            PopulationDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return new PopulationConfig(
                definition.Enemies,
                definition.Formations);
        }

        #endregion

        #region Pending Builders
        private PressureConfig BuildPressure(
            PressureDefinition definition)
        {
            return PressureConfig.Empty;
        }

        private TimelineConfig BuildTimeline(
            TimelineDefinition definition)
        {
            return TimelineConfig.Empty;
        }

        private EventConfig BuildEvents(
            ExpeditionEventDefinition definition)
        {
            return EventConfig.Empty;
        }

        private BossConfig BuildBoss(
            BossDefinition definition)
        {
            return BossConfig.Empty;
        }

        private RewardConfig BuildRewards(
            RewardDefinition definition)
        {
            return RewardConfig.Empty;
        }

        private PlayerRunConfig BuildPlayer(
            PlayerDefinition definition)
        {
            return PlayerRunConfig.Empty;
        }

        private RandomConfig BuildRandom(
            RandomDefinition definition)
        {
            return RandomConfig.Empty;
        }

        #endregion
    }
}