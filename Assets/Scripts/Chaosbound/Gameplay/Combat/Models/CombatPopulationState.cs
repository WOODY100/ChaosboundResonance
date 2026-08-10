using System;

namespace Chaosbound.Gameplay.Combat.Models
{
    public readonly struct CombatPopulationState
    {
        public int NormalCount { get; }

        public int RunnerCount { get; }

        public int TankCount { get; }

        public int TotalCount =>
            NormalCount +
            RunnerCount +
            TankCount;

        public CombatPopulationState(
            int normalCount,
            int runnerCount,
            int tankCount)
        {
            if (normalCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(normalCount));
            }

            if (runnerCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(runnerCount));
            }

            if (tankCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tankCount));
            }

            NormalCount = normalCount;
            RunnerCount = runnerCount;
            TankCount = tankCount;
        }

        public int GetCount(
            Shared.Enums.EnemyRole role)
        {
            switch (role)
            {
                case Shared.Enums.EnemyRole.Normal:
                    return NormalCount;

                case Shared.Enums.EnemyRole.Runner:
                    return RunnerCount;

                case Shared.Enums.EnemyRole.Tank:
                    return TankCount;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(role),
                        role,
                        "Unsupported Combat EnemyRole.");
            }
        }
    }
}