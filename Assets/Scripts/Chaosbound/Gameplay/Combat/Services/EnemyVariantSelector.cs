using Chaosbound.Gameplay.Combat.Models;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Combat.Services
{
    /// <summary>
    /// Selects concrete enemy variants from a resolved enemy pool.
    ///
    /// This selector does not resolve pools, determine tiers,
    /// determine roles, access Timeline, interact with Spawn,
    /// or own an RNG source.
    /// </summary>
    public sealed class EnemyVariantSelector
    {
        /// <summary>
        /// Selects the requested number of enemy variants
        /// from the supplied pool.
        ///
        /// Selection is deterministic for the current implementation.
        /// The selection strategy can later be replaced by the
        /// centralized RNG system without changing the consumer contract.
        /// </summary>
        public IReadOnlyList<EnemyVariantData> Select(
            EnemyPool pool,
            int quantity)
        {
            if (pool == null)
            {
                throw new ArgumentNullException(
                    nameof(pool));
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    quantity,
                    "Selection quantity must be greater than zero.");
            }

            if (pool.Variants.Count == 0)
            {
                throw new InvalidOperationException(
                    "Cannot select variants from an empty EnemyPool.");
            }

            List<EnemyVariantData> result =
                new List<EnemyVariantData>(
                    quantity);

            for (int i = 0;
                 i < quantity;
                 i++)
            {
                EnemyVariantData variant =
                    pool.Variants[
                        i % pool.Variants.Count];

                if (variant == null)
                {
                    throw new InvalidOperationException(
                        "EnemyPool contains a null EnemyVariantData.");
                }

                result.Add(variant);
            }

            return result;
        }
    }
}