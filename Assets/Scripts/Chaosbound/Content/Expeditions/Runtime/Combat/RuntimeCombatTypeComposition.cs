using System;

namespace Chaosbound.Content.Expeditions.Runtime.Combat
{
    public sealed class RuntimeCombatTypeComposition
    {
        public float Percentage { get; }

        public float NormalPercentage { get; }

        public float RunnerPercentage { get; }

        public float TankPercentage { get; }

        public RuntimeCombatTypeComposition(
            float percentage,
            float normalPercentage,
            float runnerPercentage,
            float tankPercentage)
        {
            Percentage =
                percentage;

            NormalPercentage =
                normalPercentage;

            RunnerPercentage =
                runnerPercentage;

            TankPercentage =
                tankPercentage;
        }
    }
}