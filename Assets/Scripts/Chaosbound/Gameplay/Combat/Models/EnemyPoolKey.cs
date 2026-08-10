using Chaosbound.Shared.Enums;
using Chaosbound.Content.Expeditions.Enums.Enemy;
using System;

namespace Chaosbound.Gameplay.Combat.Models
{
    public readonly struct EnemyPoolKey : IEquatable<EnemyPoolKey>
    {
        public EnemyTier Tier { get; }

        public EnemyRole Role { get; }

        public EnemyPoolKey(
            EnemyTier tier,
            EnemyRole role)
        {
            Tier = tier;
            Role = role;
        }

        public bool Equals(EnemyPoolKey other)
        {
            return Tier == other.Tier &&
                   Role == other.Role;
        }

        public override bool Equals(object obj)
        {
            return obj is EnemyPoolKey other &&
                   Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Tier,
                Role);
        }
    }
}