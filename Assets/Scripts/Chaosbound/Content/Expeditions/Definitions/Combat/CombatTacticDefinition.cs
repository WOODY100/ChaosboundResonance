using Chaosbound.Content.Expeditions.Definitions.Combat.SpawnPattern;
using Chaosbound.Content.Expeditions.Definitions.Combat.Replenishment;
using System;

namespace Chaosbound.Content.Expeditions.Definitions.Combat
{
    public sealed class CombatTacticDefinition
    {
        public int MaximumTarget { get; }

        public CombatTypeCompositionDefinition Melee { get; }

        public CombatTypeCompositionDefinition Ranged { get; }

        public ReplenishmentDefinition Replenishment { get; }

        public SpawnPatternDefinition SpawnPattern { get; }

        public CombatTacticDefinition(
            int maximumTarget,
            CombatTypeCompositionDefinition melee,
            CombatTypeCompositionDefinition ranged,
            ReplenishmentDefinition replenishment,
            SpawnPatternDefinition spawnPattern)
        {
            if (maximumTarget <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumTarget),
                    "Combat MaximumTarget must be greater than zero.");
            }

            if (melee == null)
            {
                throw new ArgumentNullException(
                    nameof(melee));
            }

            if (ranged == null)
            {
                throw new ArgumentNullException(
                    nameof(ranged));
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

            float combatTypeTotal =
                melee.Percentage +
                ranged.Percentage;

            const float tolerance = 0.0001f;

            if (Math.Abs(combatTypeTotal - 1f) > tolerance)
            {
                throw new ArgumentException(
                    "Combat type composition percentages must total 100%.");
            }

            MaximumTarget =
                maximumTarget;

            Melee =
                melee;

            Ranged =
                ranged;

            Replenishment =
                replenishment;

            SpawnPattern =
                spawnPattern;
        }
    }
}