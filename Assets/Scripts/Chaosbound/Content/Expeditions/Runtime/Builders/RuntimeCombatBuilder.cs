using Chaosbound.Content.Expeditions.Definitions.Combat;
using Chaosbound.Content.Expeditions.Runtime.Combat;
using Chaosbound.Content.Expeditions.Runtime.Combat.Replenishment;
using Chaosbound.Content.Expeditions.Runtime.Combat.SpawnPattern;
using System;
using System.Collections.Generic;

namespace Chaosbound.Content.Expeditions.Runtime.Builders
{
    public sealed class RuntimeCombatBuilder
    {
        public RuntimeCombatConfig BuildCombat(
            CombatDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(
                    nameof(definition));

            RuntimeCombatTargetProgression targetProgression =
                BuildTargetProgression(
                    definition.TargetProgression);

            List<RuntimeCombatTactic> tactics =
                new(definition.Tactics.Count);

            foreach (CombatTacticDefinition tactic
                in definition.Tactics)
            {
                if (tactic == null)
                {
                    throw new InvalidOperationException(
                        "CombatDefinition contains a null CombatTacticDefinition.");
                }

                RuntimeReplenishmentProfile replenishment =
                    new RuntimeReplenishmentProfile(
                        tactic.Replenishment.InitialDelay,
                        tactic.Replenishment.RecoveryInterval);

                RuntimeSpawnPatternProfile spawnPattern =
                    new RuntimeSpawnPatternProfile(
                        tactic.SpawnPattern.PerimeterPercentage,
                        tactic.SpawnPattern.FrontPercentage,
                        tactic.SpawnPattern.RearPercentage,
                        tactic.SpawnPattern.FlankPercentage);

                RuntimeCombatTypeComposition melee =
                    new RuntimeCombatTypeComposition(
                        tactic.Melee.Percentage,
                        tactic.Melee.NormalPercentage,
                        tactic.Melee.RunnerPercentage,
                        tactic.Melee.TankPercentage);

                RuntimeCombatTypeComposition ranged =
                    new RuntimeCombatTypeComposition(
                        tactic.Ranged.Percentage,
                        tactic.Ranged.NormalPercentage,
                        tactic.Ranged.RunnerPercentage,
                        tactic.Ranged.TankPercentage);

                tactics.Add(
                    new RuntimeCombatTactic(
                        tactic.MaximumTarget,
                        melee,
                        ranged,
                        replenishment,
                        spawnPattern));
            }

            return new RuntimeCombatConfig(
                targetProgression,
                tactics);
        }

        private static RuntimeCombatTargetProgression
            BuildTargetProgression(
                CombatTargetProgressionDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(
                    nameof(definition));
            }

            return new RuntimeCombatTargetProgression(
                definition.Profile);
        }
    }
}