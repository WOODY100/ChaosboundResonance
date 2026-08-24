using Chaosbound.Shared.Enums;
using System;

namespace Chaosbound.Gameplay.Combat.Models
{
    /// <summary>
    /// Represents the current number of alive enemies
    /// grouped by combat type and tactical role.
    /// </summary>
    public readonly struct CombatPopulationState
    {
        private readonly int meleeNormalCount;
        private readonly int meleeRunnerCount;
        private readonly int meleeTankCount;

        private readonly int rangedNormalCount;
        private readonly int rangedRunnerCount;
        private readonly int rangedTankCount;

        /// <summary>
        /// Gets the total number of alive enemies.
        /// </summary>
        public int TotalCount =>
            meleeNormalCount +
            meleeRunnerCount +
            meleeTankCount +
            rangedNormalCount +
            rangedRunnerCount +
            rangedTankCount;

        public CombatPopulationState(
            int meleeNormalCount,
            int meleeRunnerCount,
            int meleeTankCount,
            int rangedNormalCount,
            int rangedRunnerCount,
            int rangedTankCount)
        {
            ValidateCount(
                meleeNormalCount,
                nameof(meleeNormalCount));

            ValidateCount(
                meleeRunnerCount,
                nameof(meleeRunnerCount));

            ValidateCount(
                meleeTankCount,
                nameof(meleeTankCount));

            ValidateCount(
                rangedNormalCount,
                nameof(rangedNormalCount));

            ValidateCount(
                rangedRunnerCount,
                nameof(rangedRunnerCount));

            ValidateCount(
                rangedTankCount,
                nameof(rangedTankCount));

            this.meleeNormalCount =
                meleeNormalCount;

            this.meleeRunnerCount =
                meleeRunnerCount;

            this.meleeTankCount =
                meleeTankCount;

            this.rangedNormalCount =
                rangedNormalCount;

            this.rangedRunnerCount =
                rangedRunnerCount;

            this.rangedTankCount =
                rangedTankCount;
        }

        /// <summary>
        /// Gets the current population for a specific
        /// combat type and tactical role.
        /// </summary>
        public int GetCount(
            EnemyCombatType combatType,
            EnemyRole role)
        {
            switch (combatType)
            {
                case EnemyCombatType.Melee:

                    switch (role)
                    {
                        case EnemyRole.Normal:
                            return meleeNormalCount;

                        case EnemyRole.Runner:
                            return meleeRunnerCount;

                        case EnemyRole.Tank:
                            return meleeTankCount;

                        default:
                            throw new ArgumentOutOfRangeException(
                                nameof(role),
                                role,
                                "Unsupported Combat EnemyRole.");
                    }

                case EnemyCombatType.Ranged:

                    switch (role)
                    {
                        case EnemyRole.Normal:
                            return rangedNormalCount;

                        case EnemyRole.Runner:
                            return rangedRunnerCount;

                        case EnemyRole.Tank:
                            return rangedTankCount;

                        default:
                            throw new ArgumentOutOfRangeException(
                                nameof(role),
                                role,
                                "Unsupported Combat EnemyRole.");
                    }

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(combatType),
                        combatType,
                        "Unsupported EnemyCombatType.");
            }
        }

        private static void ValidateCount(
            int count,
            string parameterName)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(
                    parameterName);
            }
        }
    }
}