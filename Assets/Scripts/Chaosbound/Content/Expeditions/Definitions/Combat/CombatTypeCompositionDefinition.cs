using System;

namespace Chaosbound.Content.Expeditions.Definitions.Combat
{
    public sealed class CombatTypeCompositionDefinition
    {
        public float Percentage { get; }

        public float NormalPercentage { get; }

        public float RunnerPercentage { get; }

        public float TankPercentage { get; }

        public CombatTypeCompositionDefinition(
            float percentage,
            float normalPercentage,
            float runnerPercentage,
            float tankPercentage)
        {
            ValidatePercentage(
                percentage,
                nameof(percentage));

            ValidatePercentage(
                normalPercentage,
                nameof(normalPercentage));

            ValidatePercentage(
                runnerPercentage,
                nameof(runnerPercentage));

            ValidatePercentage(
                tankPercentage,
                nameof(tankPercentage));

            float roleTotal =
                normalPercentage +
                runnerPercentage +
                tankPercentage;

            const float tolerance = 0.0001f;

            if (Math.Abs(roleTotal - 1f) > tolerance)
            {
                throw new ArgumentException(
                    "Combat role composition percentages must total 100%.");
            }

            Percentage = percentage;

            NormalPercentage =
                normalPercentage;

            RunnerPercentage =
                runnerPercentage;

            TankPercentage =
                tankPercentage;
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