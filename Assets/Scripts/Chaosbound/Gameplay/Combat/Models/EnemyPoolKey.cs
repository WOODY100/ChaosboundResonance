using Chaosbound.Shared.Enums;
using System;

namespace Chaosbound.Gameplay.Combat.Models
{
    public readonly struct EnemyPoolKey : IEquatable<EnemyPoolKey>
    {
        public EnemyTier Tier { get; }

        public EnemyCombatType CombatType { get; }

        public EnemyRole Role { get; }

        public EnemyPoolKey(
            EnemyTier tier,
            EnemyCombatType combatType,
            EnemyRole role)
        {
            Tier = tier;
            CombatType = combatType;
            Role = role;
        }

        public bool Equals(
            EnemyPoolKey other)
        {
            return Tier == other.Tier &&
                   CombatType == other.CombatType &&
                   Role == other.Role;
        }

        public override bool Equals(
            object obj)
        {
            return obj is EnemyPoolKey other &&
                   Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Tier,
                CombatType,
                Role);
        }
    }
}