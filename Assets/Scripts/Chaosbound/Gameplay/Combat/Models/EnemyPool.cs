using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Combat.Models
{
    public sealed class EnemyPool
    {
        public EnemyPoolKey Key { get; }

        public IReadOnlyList<EnemyVariantData> Variants { get; }

        public EnemyPool(
            EnemyPoolKey key,
            IReadOnlyList<EnemyVariantData> variants)
        {
            if (variants == null)
                throw new ArgumentNullException(nameof(variants));

            Key = key;
            Variants =
                new List<EnemyVariantData>(variants);
        }
    }
}