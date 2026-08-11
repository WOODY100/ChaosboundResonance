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

            int normalCount = 0;
            int runnerCount = 0;
            int tankCount = 0;

            foreach (CombatRuntimeCompositionEntry entry
                in composition.Entries)
            {
                if (entry == null)
                    continue;

                EnemyRole role =
                    CombatRoleResolver.Resolve(
                        entry.Variant);

                switch (role)
                {
                    case EnemyRole.Normal:
                        normalCount += entry.AliveCount;
                        break;

                    case EnemyRole.Runner:
                        runnerCount += entry.AliveCount;
                        break;

                    case EnemyRole.Tank:
                        tankCount += entry.AliveCount;
                        break;

                    default:
                        throw new InvalidOperationException(
                            $"Unsupported Combat EnemyRole '{role}'.");
                }
            }

            return new CombatPopulationState(
                normalCount,
                runnerCount,
                tankCount);
        }
    }
}