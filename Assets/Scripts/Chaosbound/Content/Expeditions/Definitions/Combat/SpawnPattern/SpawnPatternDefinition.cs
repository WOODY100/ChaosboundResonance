using System;

namespace Chaosbound.Content.Expeditions.Definitions.Combat.SpawnPattern
{
    public sealed class SpawnPatternDefinition
    {
        public float PerimeterPercentage { get; }

        public float FrontPercentage { get; }

        public float RearPercentage { get; }

        public float FlankPercentage { get; }

        public SpawnPatternDefinition(
            float perimeterPercentage,
            float frontPercentage,
            float rearPercentage,
            float flankPercentage)
        {
            ValidatePercentage(
                perimeterPercentage,
                nameof(perimeterPercentage));

            ValidatePercentage(
                frontPercentage,
                nameof(frontPercentage));

            ValidatePercentage(
                rearPercentage,
                nameof(rearPercentage));

            ValidatePercentage(
                flankPercentage,
                nameof(flankPercentage));

            float total =
                perimeterPercentage +
                frontPercentage +
                rearPercentage +
                flankPercentage;

            const float tolerance = 0.0001f;

            if (Math.Abs(total - 1f) > tolerance)
            {
                throw new ArgumentException(
                    "Spawn pattern percentages must total 100%.");
            }

            PerimeterPercentage =
                perimeterPercentage;

            FrontPercentage =
                frontPercentage;

            RearPercentage =
                rearPercentage;

            FlankPercentage =
                flankPercentage;
        }

        private static void ValidatePercentage(
            float value,
            string parameterName)
        {
            if (value < 0f || value > 1f)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    "Spawn pattern percentage must be between 0 and 1.");
            }
        }
    }
}