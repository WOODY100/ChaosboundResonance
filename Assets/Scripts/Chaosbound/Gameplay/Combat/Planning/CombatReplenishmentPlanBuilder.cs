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
    /// The enemy tier is temporarily fixed to Tier1 until
    /// the Expedition Progression/Timeline system provides
    /// the active tier.
    /// </summary>
    public sealed class CombatReplenishmentPlanBuilder
    {
        /// <summary>
        /// Builds a replenishment plan from the supplied decision.
        /// </summary>
        public CombatReplenishmentPlan Build(
            ReplenishmentDecision decision)
        {
            if (!decision.IsRequired)
            {
                throw new InvalidOperationException(
                    "Cannot build a replenishment plan from a decision " +
                    "that does not require replenishment.");
            }

            // Temporary until Expedition Progression/Timeline
            // provides the active EnemyTier.
            EnemyTier tier =
                EnemyTier.Tier1;

            return new CombatReplenishmentPlan(
                decision.Role,
                decision.Quantity,
                tier);
        }
    }
}