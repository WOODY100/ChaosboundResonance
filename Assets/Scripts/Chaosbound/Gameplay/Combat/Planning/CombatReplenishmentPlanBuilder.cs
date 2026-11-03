using Chaosbound.Gameplay.Combat.Decisions;
using Chaosbound.Gameplay.Combat.Planning;
using Chaosbound.Shared.Enums;
using System;

namespace Chaosbound.Gameplay.Combat.Services
{
    /// <summary>
    /// Builds a concrete CombatReplenishmentPlan from
    /// a replenishment decision.
    ///
    /// The active enemy tier is supplied by Combat runtime state.
    /// </summary>
    public sealed class CombatReplenishmentPlanBuilder
    {
        /// <summary>
        /// Builds a replenishment plan from the supplied decision.
        /// </summary>
        public CombatReplenishmentPlan Build(
            ReplenishmentDecision decision,
            EnemyTier activeTier)
        {
            if (!decision.IsRequired)
            {
                throw new InvalidOperationException(
                    "Cannot build a replenishment plan from a decision " +
                    "that does not require replenishment.");
            }

            return new CombatReplenishmentPlan(
                decision.CombatType,
                decision.Role,
                decision.Quantity,
                activeTier);
        }
    }
}