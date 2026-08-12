using Chaosbound.Content.Expeditions.Authoring.Combat;
using Chaosbound.Content.Expeditions.Authoring.Combat.Replenishment;
using Chaosbound.Content.Expeditions.Authoring.Combat.SpawnPattern;
using Chaosbound.Content.Expeditions.Definitions.Combat;
using Chaosbound.Content.Expeditions.Definitions.Combat.Replenishment;
using Chaosbound.Content.Expeditions.Definitions.Combat.SpawnPattern;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Builders.Combat
{
    public static class CombatBuilder
    {
        public static CombatDefinition Build(
            CombatAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            CombatTargetProgressionDefinition targetProgression =
                BuildTargetProgression(
                    authoring.TargetProgression);

            List<CombatTacticDefinition> tactics =
                BuildTactics(authoring.Tactics);

            return new CombatDefinition(
                targetProgression,
                tactics);
        }

        private static List<CombatTacticDefinition> BuildTactics(
            IReadOnlyList<CombatTacticAuthoring> authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            List<CombatTacticDefinition> result =
                new(authoring.Count);

            foreach (CombatTacticAuthoring tactic in authoring)
            {
                if (tactic == null)
                {
                    throw new InvalidOperationException(
                        "CombatAuthoring contains a null CombatTacticAuthoring.");
                }

                SpawnPatternDefinition spawnPattern =
                    BuildSpawnPattern(
                        tactic.SpawnPattern);

                ReplenishmentDefinition replenishment =
                BuildReplenishment(tactic.Replenishment);

                result.Add(
                    new CombatTacticDefinition(
                        tactic.MaximumTarget,
                        tactic.NormalPercentage,
                        tactic.RunnerPercentage,
                        tactic.TankPercentage,
                        replenishment,
                        spawnPattern));
            }

            return result;
        }

        private static ReplenishmentDefinition BuildReplenishment(
            ReplenishmentAuthoring authoring)
        {
            if (authoring == null)
            {
                throw new ArgumentNullException(nameof(authoring));
            }

            return new ReplenishmentDefinition(
                authoring.InitialDelay,
                authoring.RecoveryInterval);
        }

        private static SpawnPatternDefinition BuildSpawnPattern(
            SpawnPatternAuthoring authoring)
        {
            if (authoring == null)
            {
                throw new ArgumentNullException(
                    nameof(authoring));
            }

            return new SpawnPatternDefinition(
                authoring.PerimeterPercentage,
                authoring.FrontPercentage,
                authoring.RearPercentage,
                authoring.FlankPercentage);
        }

        private static CombatTargetProgressionDefinition
            BuildTargetProgression(
        CombatTargetProgressionAuthoring authoring)
        {
            if (authoring == null)
            {
                throw new ArgumentNullException(
                    nameof(authoring));
            }

            if (authoring.Profile == null)
            {
                throw new InvalidOperationException(
                    "CombatTargetProgressionAuthoring requires a profile.");
            }

            return new CombatTargetProgressionDefinition(
                authoring.Profile);
        }
    }
}