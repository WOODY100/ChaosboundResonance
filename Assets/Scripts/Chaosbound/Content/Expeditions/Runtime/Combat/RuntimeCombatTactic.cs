using Chaosbound.Content.Expeditions.Runtime.Combat.SpawnPattern;
using Chaosbound.Content.Expeditions.Runtime.Combat.Replenishment;
using System;

namespace Chaosbound.Content.Expeditions.Runtime.Combat
{
    public sealed class RuntimeCombatTactic
    {
        public int MaximumTarget { get; }

        public RuntimeCombatTypeComposition Melee { get; }

        public RuntimeCombatTypeComposition Ranged { get; }

        public RuntimeReplenishmentProfile Replenishment { get; }

        public RuntimeSpawnPatternProfile SpawnPattern { get; }

        public RuntimeCombatTactic(
            int maximumTarget,
            RuntimeCombatTypeComposition melee,
            RuntimeCombatTypeComposition ranged,
            RuntimeReplenishmentProfile replenishment,
            RuntimeSpawnPatternProfile spawnPattern)
        {
            MaximumTarget =
                maximumTarget;

            Melee =
                melee
                ?? throw new ArgumentNullException(
                    nameof(melee));

            Ranged =
                ranged
                ?? throw new ArgumentNullException(
                    nameof(ranged));

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