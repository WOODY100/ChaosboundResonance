using Chaosbound.Content.Expeditions.Runtime.Combat.SpawnPattern;
using Chaosbound.Content.Expeditions.Runtime.Combat.Replenishment;
using System;

namespace Chaosbound.Content.Expeditions.Runtime.Combat
{
    public sealed class RuntimeCombatTactic
    {
        public int Target { get; }

        public float NormalPercentage { get; }

        public float RunnerPercentage { get; }

        public float TankPercentage { get; }

        public RuntimeReplenishmentProfile Replenishment { get; }

        public RuntimeSpawnPatternProfile SpawnPattern { get; }

        public RuntimeCombatTactic(
            int target,
            float normalPercentage,
            float runnerPercentage,
            float tankPercentage,
            RuntimeReplenishmentProfile replenishment,
            RuntimeSpawnPatternProfile spawnPattern)
        {
            Target = target;

            NormalPercentage = normalPercentage;

            RunnerPercentage = runnerPercentage;

            TankPercentage = tankPercentage;

            Replenishment =
                replenishment
                ?? throw new ArgumentNullException(
                    nameof(replenishment));

            SpawnPattern =
                spawnPattern
                ?? throw new ArgumentNullException(
                    nameof(spawnPattern));
        }
    }
}