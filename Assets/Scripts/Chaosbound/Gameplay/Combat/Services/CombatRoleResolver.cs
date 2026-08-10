using Chaosbound.Shared.Enums;
using System;

namespace Chaosbound.Gameplay.Combat.Services
{
    public static class CombatRoleResolver
    {
        public static EnemyRole Resolve(
            EnemyVariantData variant)
        {
            if (variant == null)
            {
                throw new ArgumentNullException(
                    nameof(variant));
            }

            if (variant.Roles == null ||
                variant.Roles.Count != 1)
            {
                throw new InvalidOperationException(
                    $"Enemy variant '{variant.name}' " +
                    "must define exactly one EnemyRole " +
                    "for Combat V1.");
            }

            EnemyRole role =
                variant.Roles[0];

            switch (role)
            {
                case EnemyRole.Normal:
                case EnemyRole.Runner:
                case EnemyRole.Tank:
                    return role;

                default:
                    throw new InvalidOperationException(
                        $"Enemy variant '{variant.name}' " +
                        $"uses unsupported Combat EnemyRole '{role}'.");
            }
        }
    }
}