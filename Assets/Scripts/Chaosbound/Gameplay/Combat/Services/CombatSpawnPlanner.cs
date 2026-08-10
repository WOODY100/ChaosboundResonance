using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Gameplay.Combat.Models;
using Chaosbound.Gameplay.Combat.Planning;
using System;

namespace Chaosbound.Gameplay.Combat.Services
{
    /// <summary>
    /// Resolves a CombatReplenishmentPlan into a concrete
    /// CombatSpawnPlan.
    ///
    /// This planner coordinates pool resolution, variant
    /// selection and spawn-plan construction.
    ///
    /// It does not decide the combat role, quantity or tier.
    /// It does not create SpawnRequests.
    /// It does not interact with Spawn Runtime.
    /// </summary>
    public sealed class CombatSpawnPlanner
    {
        private readonly EnemyPoolResolver
            poolResolver;

        private readonly EnemyVariantSelector
            variantSelector;

        private readonly CombatSpawnPlanBuilder
            spawnPlanBuilder;

        public CombatSpawnPlanner(
            EnemyPoolResolver poolResolver,
            EnemyVariantSelector variantSelector,
            CombatSpawnPlanBuilder spawnPlanBuilder)
        {
            this.poolResolver =
                poolResolver
                ?? throw new ArgumentNullException(
                    nameof(poolResolver));

            this.variantSelector =
                variantSelector
                ?? throw new ArgumentNullException(
                    nameof(variantSelector));

            this.spawnPlanBuilder =
                spawnPlanBuilder
                ?? throw new ArgumentNullException(
                    nameof(spawnPlanBuilder));
        }

        /// <summary>
        /// Resolves a replenishment plan into a concrete
        /// combat spawn plan.
        /// </summary>
        public CombatSpawnPlan Build(
            CombatReplenishmentPlan replenishmentPlan,
            RuntimeEnemyConfig enemyConfig)
        {
            if (replenishmentPlan == null)
            {
                throw new ArgumentNullException(
                    nameof(replenishmentPlan));
            }

            if (enemyConfig == null)
            {
                throw new ArgumentNullException(
                    nameof(enemyConfig));
            }

            EnemyPool pool =
                poolResolver.Resolve(
                    enemyConfig,
                    replenishmentPlan.Tier,
                    replenishmentPlan.Role);

            var variants =
                variantSelector.Select(
                    pool,
                    replenishmentPlan.Quantity);

            return spawnPlanBuilder.Build(
                variants);
        }
    }
}