using Chaosbound.Shared.Enums;
using Chaosbound.Content.Expeditions.Runtime.Enemy;
using Chaosbound.Gameplay.Combat.Models;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Combat.Services
{
    public sealed class EnemyPoolResolver
    {
        public EnemyPool Resolve(
            RuntimeEnemyConfig enemyConfig,
            EnemyTier tier,
            EnemyCombatType combatType,
            EnemyRole role)
        {
            if (enemyConfig == null)
                throw new ArgumentNullException(
                    nameof(enemyConfig));

            List<EnemyVariantData> variants =
                new();

            foreach (EnemyVariantData enemy
                in enemyConfig.Enemies)
            {
                if (enemy == null)
                    continue;

                if (enemy.Category != EnemyCategory.Normal)
                    continue;

                if (enemy.Tier != tier)
                    continue;

                if (enemy.CombatType != combatType)
                    continue;

                EnemyRole combatRole =
                    CombatRoleResolver.Resolve(enemy);

                if (combatRole != role)
                    continue;

                variants.Add(enemy);
            }

            EnemyPoolKey key =
                new EnemyPoolKey(
                    tier,
                    combatType,
                    role);

            if (variants.Count == 0)
            {
                throw new InvalidOperationException(
                    $"No enemy variants available for " +
                    $"Tier={tier}, " +
                    $"CombatType={combatType}, " +
                    $"Role={role}.");
            }

            return new EnemyPool(
                key,
                variants);
        }
    }
}