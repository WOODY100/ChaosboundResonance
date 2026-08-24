using Chaosbound.Gameplay.Combat.Models;
using Chaosbound.Gameplay.Combat.Services;
using Chaosbound.Shared.Enums;
using System;

namespace Chaosbound.Gameplay.Combat.Runtime.Composition
{
    public static class CombatPopulationAdapter
    {
        public static CombatPopulationState Build(
            CombatRuntimeComposition composition)
        {
            if (composition == null)
            {
                throw new ArgumentNullException(
                    nameof(composition));
            }

            int meleeNormalCount = 0;
            int meleeRunnerCount = 0;
            int meleeTankCount = 0;

            int rangedNormalCount = 0;
            int rangedRunnerCount = 0;
            int rangedTankCount = 0;

            foreach (
                CombatRuntimeCompositionEntry entry
                in composition.Entries)
            {
                if (entry == null)
                    continue;

                EnemyCombatType combatType =
                    entry.Variant.CombatType;

                EnemyRole role =
                    CombatRoleResolver.Resolve(
                        entry.Variant);

                switch (combatType)
                {
                    case EnemyCombatType.Melee:

                        switch (role)
                        {
                            case EnemyRole.Normal:
                                meleeNormalCount +=
                                    entry.AliveCount;
                                break;

                            case EnemyRole.Runner:
                                meleeRunnerCount +=
                                    entry.AliveCount;
                                break;

                            case EnemyRole.Tank:
                                meleeTankCount +=
                                    entry.AliveCount;
                                break;

                            default:
                                throw new InvalidOperationException(
                                    $"Unsupported Combat EnemyRole '{role}'.");
                        }

                        break;

                    case EnemyCombatType.Ranged:

                        switch (role)
                        {
                            case EnemyRole.Normal:
                                rangedNormalCount +=
                                    entry.AliveCount;
                                break;

                            case EnemyRole.Runner:
                                rangedRunnerCount +=
                                    entry.AliveCount;
                                break;

                            case EnemyRole.Tank:
                                rangedTankCount +=
                                    entry.AliveCount;
                                break;

                            default:
                                throw new InvalidOperationException(
                                    $"Unsupported Combat EnemyRole '{role}'.");
                        }

                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Unsupported EnemyCombatType '{combatType}'.");
                }
            }

            return new CombatPopulationState(
                meleeNormalCount,
                meleeRunnerCount,
                meleeTankCount,
                rangedNormalCount,
                rangedRunnerCount,
                rangedTankCount);
        }
    }
}