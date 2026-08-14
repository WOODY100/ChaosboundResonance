using Chaosbound.Content.Expeditions.Runtime.Combat;
using Chaosbound.Gameplay.Combat.Models;
using Chaosbound.Gameplay.Combat.Results;
using Chaosbound.Shared.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;

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
    /// - interact with Threat Budget or Pressure;
    /// - evaluate target progression.
    ///
    /// Its only responsibility is to translate the supplied
    /// target and role percentages into a deterministic
    /// CombatComposition.
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

            ValidatePercentages(tactic);

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
            List<RoleAllocation> allocations =
                new List<RoleAllocation>();

            AddAllocation(
                allocations,
                EnemyRole.Normal,
                target,
                tactic.NormalPercentage);

            AddAllocation(
                allocations,
                EnemyRole.Runner,
                target,
                tactic.RunnerPercentage);

            AddAllocation(
                allocations,
                EnemyRole.Tank,
                target,
                tactic.TankPercentage);

            DistributeRemainingUnits(
                allocations,
                target);

            List<CombatRuntimeCompositionEntry> entries =
                new List<CombatRuntimeCompositionEntry>();

            foreach (RoleAllocation allocation in allocations)
            {
                if (allocation.Quantity <= 0)
                    continue;

                entries.Add(
                    new CombatRuntimeCompositionEntry(
                        allocation.Role,
                        allocation.Quantity));
            }

            return new CombatComposition(entries);
        }

        private static void AddAllocation(
            List<RoleAllocation> allocations,
            EnemyRole role,
            int target,
            float percentage)
        {
            float exactQuantity =
                target * percentage;

            int baseQuantity =
                (int)Math.Floor(exactQuantity);

            float remainder =
                exactQuantity - baseQuantity;

            allocations.Add(
                new RoleAllocation(
                    role,
                    baseQuantity,
                    remainder));
        }

        private static void DistributeRemainingUnits(
            List<RoleAllocation> allocations,
            int target)
        {
            int allocated =
                0;

            foreach (RoleAllocation allocation in allocations)
            {
                allocated +=
                    allocation.Quantity;
            }

            int remaining =
                target - allocated;

            allocations.Sort(
                (a, b) =>
                    b.Remainder.CompareTo(
                        a.Remainder));

            for (int i = 0;
                 i < remaining;
                 i++)
            {
                allocations[i % allocations.Count]
                    .Quantity++;
            }

            allocations.Sort(
                (a, b) =>
                    ((int)a.Role).CompareTo(
                        (int)b.Role));
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

            float total =
                tactic.NormalPercentage +
                tactic.RunnerPercentage +
                tactic.TankPercentage;

            if (tactic.MaximumTarget < 3)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tactic.MaximumTarget),
                    tactic.MaximumTarget,
                    "Combat MaximumTarget must be at least 3.");
            }

            if (tactic.NormalPercentage < 0f ||
                tactic.RunnerPercentage < 0f ||
                tactic.TankPercentage < 0f)
            {
                throw new InvalidOperationException(
                    "Combat role percentages cannot be negative.");
            }

            if (Math.Abs(total - 1f) > tolerance)
            {
                throw new InvalidOperationException(
                    $"Combat role percentages must total 1. " +
                    $"Current total={total}.");
            }
        }

        /// <summary>
        /// Internal mutable allocation used only while
        /// constructing the immutable CombatComposition.
        /// </summary>
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
                Role = role;
                Quantity = quantity;
                Remainder = remainder;
            }
        }
    }
}