using Chaosbound.Content.Expeditions.Definitions.Combat.SpawnPattern;
using Chaosbound.Content.Expeditions.Definitions.Combat.Replenishment;
using System;

namespace Chaosbound.Content.Expeditions.Definitions.Combat
{
    public sealed class CombatTacticDefinition
    {
        public int Target { get; }

        public float NormalPercentage { get; }

        public float RunnerPercentage { get; }

        public float TankPercentage { get; }

        public ReplenishmentDefinition Replenishment { get; }

        public SpawnPatternDefinition SpawnPattern { get; }

        public CombatTacticDefinition(
            int target,
            float normalPercentage,
            float runnerPercentage,
            float tankPercentage,
            ReplenishmentDefinition replenishment,
            SpawnPatternDefinition spawnPattern)
        {
            if (target <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(target),
                    "Combat Target must be greater than zero.");
            }

            if (replenishment == null)
            {
                throw new ArgumentNullException(
                    nameof(replenishment));
            }

            if (spawnPattern == null)
            {
                throw new ArgumentNullException(
                    nameof(spawnPattern));
            }

            ValidatePercentage(
                normalPercentage,
                nameof(normalPercentage));

            ValidatePercentage(
                runnerPercentage,
                nameof(runnerPercentage));

            ValidatePercentage(
                tankPercentage,
                nameof(tankPercentage));

            float total =
                normalPercentage +
                runnerPercentage +
                tankPercentage;

            const float tolerance = 0.0001f;

            if (Math.Abs(total - 1f) > tolerance)
            {
                throw new ArgumentException(
                    "Combat composition percentages must total 100%.");
            }

            Target = target;

            NormalPercentage = normalPercentage;
            RunnerPercentage = runnerPercentage;
            TankPercentage = tankPercentage;
            Replenishment = replenishment;
            SpawnPattern = spawnPattern;
        }

        private static void ValidatePercentage(
            float value,
            string parameterName)
        {
            if (value < 0f || value > 1f)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    "Composition percentage must be between 0 and 1.");
            }
        }
    }
}