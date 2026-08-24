using Chaosbound.Content.Expeditions.Runtime.Combat;
using Chaosbound.Gameplay.Combat.Models;
using Chaosbound.Gameplay.Combat.Results;
using Chaosbound.Shared.Enums;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Combat.Services
{
    /// <summary>
    /// Resolves a combat tactic into a desired combat composition.
    ///
    /// This solver does not:
    /// - select concrete enemy variants;
    /// - access enemy pools;
    /// - use RNG;
    /// - manage replenishment;
    /// - create SpawnRequests;
    /// - interact with Spawn Runtime;
    /// - evaluate target progression.
    ///
    /// Its only responsibility is to translate the supplied
    /// target into a deterministic CombatType + Role composition.
    ///
    /// Composition is resolved hierarchically:
    ///
    /// 1. CombatType distribution:
    ///    Melee / Ranged
    ///
    /// 2. Role distribution inside each CombatType:
    ///    Normal / Runner / Tank
    /// </summary>
    public sealed class CombatSolver
    {
        /// <summary>
        /// Resolves the supplied combat tactic using the
        /// already evaluated combat target.
        /// </summary>
        public CombatResult Solve(
            RuntimeCombatTactic tactic,
            int target)
        {
            if (tactic == null)
            {
                throw new ArgumentNullException(
                    nameof(tactic));
            }

            ValidateTarget(
                tactic,
                target);

            ValidatePercentages(
                tactic);

            CombatComposition composition =
                BuildComposition(
                    tactic,
                    target);

            return new CombatResult(
                composition,
                tactic.SpawnPattern);
        }

        private CombatComposition BuildComposition(
            RuntimeCombatTactic tactic,
            int target)
        {
            List<CombatTypeAllocation> typeAllocations =
                BuildCombatTypeAllocations(
                    tactic,
                    target);

            List<CombatRuntimeCompositionEntry> entries =
                new List<CombatRuntimeCompositionEntry>();

            foreach (
                CombatTypeAllocation typeAllocation
                in typeAllocations)
            {
                if (typeAllocation.Quantity <= 0)
                    continue;

                RuntimeCombatTypeComposition typeComposition =
                    GetTypeComposition(
                        tactic,
                        typeAllocation.CombatType);

                List<RoleAllocation> roleAllocations =
                    BuildRoleAllocations(
                        typeComposition,
                        typeAllocation.Quantity);

                foreach (
                    RoleAllocation roleAllocation
                    in roleAllocations)
                {
                    if (roleAllocation.Quantity <= 0)
                        continue;

                    entries.Add(
                        new CombatRuntimeCompositionEntry(
                            typeAllocation.CombatType,
                            roleAllocation.Role,
                            roleAllocation.Quantity));
                }
            }

            return new CombatComposition(
                entries);
        }

        private static List<CombatTypeAllocation>
            BuildCombatTypeAllocations(
                RuntimeCombatTactic tactic,
                int target)
        {
            List<CombatTypeAllocation> allocations =
                new List<CombatTypeAllocation>();

            AddCombatTypeAllocation(
                allocations,
                EnemyCombatType.Melee,
                target,
                tactic.Melee.Percentage);

            AddCombatTypeAllocation(
                allocations,
                EnemyCombatType.Ranged,
                target,
                tactic.Ranged.Percentage);

            DistributeRemainingUnits(
                allocations,
                target);

            return allocations;
        }

        private static void AddCombatTypeAllocation(
            List<CombatTypeAllocation> allocations,
            EnemyCombatType combatType,
            int target,
            float percentage)
        {
            float exactQuantity =
                target * percentage;

            int baseQuantity =
                (int)Math.Floor(
                    exactQuantity);

            float remainder =
                exactQuantity -
                baseQuantity;

            allocations.Add(
                new CombatTypeAllocation(
                    combatType,
                    baseQuantity,
                    remainder));
        }

        private static List<RoleAllocation>
            BuildRoleAllocations(
                RuntimeCombatTypeComposition composition,
                int target)
        {
            List<RoleAllocation> allocations =
                new List<RoleAllocation>();

            AddRoleAllocation(
                allocations,
                EnemyRole.Normal,
                target,
                composition.NormalPercentage);

            AddRoleAllocation(
                allocations,
                EnemyRole.Runner,
                target,
                composition.RunnerPercentage);

            AddRoleAllocation(
                allocations,
                EnemyRole.Tank,
                target,
                composition.TankPercentage);

            DistributeRemainingUnits(
                allocations,
                target);

            allocations.Sort(
                (a, b) =>
                    ((int)a.Role).CompareTo(
                        (int)b.Role));

            return allocations;
        }

        private static void AddRoleAllocation(
            List<RoleAllocation> allocations,
            EnemyRole role,
            int target,
            float percentage)
        {
            float exactQuantity =
                target * percentage;

            int baseQuantity =
                (int)Math.Floor(
                    exactQuantity);

            float remainder =
                exactQuantity -
                baseQuantity;

            allocations.Add(
                new RoleAllocation(
                    role,
                    baseQuantity,
                    remainder));
        }

        private static void DistributeRemainingUnits(
            List<CombatTypeAllocation> allocations,
            int target)
        {
            int allocated =
                0;

            foreach (
                CombatTypeAllocation allocation
                in allocations)
            {
                allocated +=
                    allocation.Quantity;
            }

            int remaining =
                target -
                allocated;

            if (remaining <= 0)
                return;

            allocations.Sort(
                (a, b) =>
                {
                    int remainderComparison =
                        b.Remainder.CompareTo(
                            a.Remainder);

                    if (remainderComparison != 0)
                        return remainderComparison;

                    return ((int)a.CombatType).CompareTo(
                        (int)b.CombatType);
                });

            for (int i = 0;
                 i < remaining;
                 i++)
            {
                allocations[
                    i % allocations.Count]
                    .Quantity++;
            }

            allocations.Sort(
                (a, b) =>
                    ((int)a.CombatType).CompareTo(
                        (int)b.CombatType));
        }

        private static void DistributeRemainingUnits(
            List<RoleAllocation> allocations,
            int target)
        {
            int allocated =
                0;

            foreach (
                RoleAllocation allocation
                in allocations)
            {
                allocated +=
                    allocation.Quantity;
            }

            int remaining =
                target -
                allocated;

            if (remaining <= 0)
                return;

            allocations.Sort(
                (a, b) =>
                {
                    int remainderComparison =
                        b.Remainder.CompareTo(
                            a.Remainder);

                    if (remainderComparison != 0)
                        return remainderComparison;

                    return ((int)a.Role).CompareTo(
                        (int)b.Role);
                });

            for (int i = 0;
                 i < remaining;
                 i++)
            {
                allocations[
                    i % allocations.Count]
                    .Quantity++;
            }
        }

        private static RuntimeCombatTypeComposition
            GetTypeComposition(
                RuntimeCombatTactic tactic,
                EnemyCombatType combatType)
        {
            switch (combatType)
            {
                case EnemyCombatType.Melee:
                    return tactic.Melee;

                case EnemyCombatType.Ranged:
                    return tactic.Ranged;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(combatType),
                        combatType,
                        "Unsupported EnemyCombatType.");
            }
        }

        private static void ValidateTarget(
            RuntimeCombatTactic tactic,
            int target)
        {
            if (target < 3)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(target),
                    target,
                    "Combat target must be at least 3.");
            }

            if (target > tactic.MaximumTarget)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(target),
                    target,
                    "Combat target cannot exceed the tactic MaximumTarget.");
            }
        }

        private static void ValidatePercentages(
            RuntimeCombatTactic tactic)
        {
            const float tolerance =
                0.0001f;

            if (tactic.MaximumTarget < 3)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tactic.MaximumTarget),
                    tactic.MaximumTarget,
                    "Combat MaximumTarget must be at least 3.");
            }

            ValidateTypeComposition(
                tactic.Melee,
                nameof(tactic.Melee));

            ValidateTypeComposition(
                tactic.Ranged,
                nameof(tactic.Ranged));

            float combatTypeTotal =
                tactic.Melee.Percentage +
                tactic.Ranged.Percentage;

            if (Math.Abs(
                    combatTypeTotal - 1f) >
                tolerance)
            {
                throw new InvalidOperationException(
                    "Combat type percentages must total 1. " +
                    $"Current total={combatTypeTotal}.");
            }
        }

        private static void ValidateTypeComposition(
            RuntimeCombatTypeComposition composition,
            string parameterName)
        {
            const float tolerance =
                0.0001f;

            if (composition == null)
            {
                throw new ArgumentNullException(
                    parameterName);
            }

            if (composition.Percentage < 0f ||
                composition.Percentage > 1f)
            {
                throw new InvalidOperationException(
                    $"{parameterName} percentage must be between 0 and 1.");
            }

            if (composition.NormalPercentage < 0f ||
                composition.NormalPercentage > 1f)
            {
                throw new InvalidOperationException(
                    $"{parameterName}.NormalPercentage must be between 0 and 1.");
            }

            if (composition.RunnerPercentage < 0f ||
                composition.RunnerPercentage > 1f)
            {
                throw new InvalidOperationException(
                    $"{parameterName}.RunnerPercentage must be between 0 and 1.");
            }

            if (composition.TankPercentage < 0f ||
                composition.TankPercentage > 1f)
            {
                throw new InvalidOperationException(
                    $"{parameterName}.TankPercentage must be between 0 and 1.");
            }

            float roleTotal =
                composition.NormalPercentage +
                composition.RunnerPercentage +
                composition.TankPercentage;

            if (Math.Abs(
                    roleTotal - 1f) >
                tolerance)
            {
                throw new InvalidOperationException(
                    $"{parameterName} role percentages must total 1. " +
                    $"Current total={roleTotal}.");
            }
        }

        private sealed class CombatTypeAllocation
        {
            public EnemyCombatType CombatType { get; }

            public int Quantity { get; set; }

            public float Remainder { get; }

            public CombatTypeAllocation(
                EnemyCombatType combatType,
                int quantity,
                float remainder)
            {
                CombatType =
                    combatType;

                Quantity =
                    quantity;

                Remainder =
                    remainder;
            }
        }

        private sealed class RoleAllocation
        {
            public EnemyRole Role { get; }

            public int Quantity { get; set; }

            public float Remainder { get; }

            public RoleAllocation(
                EnemyRole role,
                int quantity,
                float remainder)
            {
                Role =
                    role;

                Quantity =
                    quantity;

                Remainder =
                    remainder;
            }
        }
    }
}