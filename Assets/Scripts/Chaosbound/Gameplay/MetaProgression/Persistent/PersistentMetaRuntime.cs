using System;
using UnityEngine;

namespace Chaosbound.Gameplay.MetaProgression.Persistent
{
    public sealed class PersistentMetaRuntime : MonoBehaviour
    {
        private const int BaseExperience = 2000;

        private const float ExperienceGrowthMultiplier = 1.20f;

        private PersistentMetaState state;

        public event Action MetaProgressionChanged;

        public PersistentMetaState State =>
            state;

        public int ExperienceRequiredForNextLevel =>
            CalculateExperienceRequiredForNextLevel();

        private void Awake()
        {
            state = new PersistentMetaState();
        }

        public void AddMetaExperience(int amount)
        {
            EnsureInitialized();

            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Meta experience cannot be negative.");
            }

            if (amount == 0)
                return;

            if (!state.CanAddExperience(amount))
            {
                throw new InvalidOperationException(
                    "Cannot add meta experience because " +
                    "the resulting experience would exceed " +
                    "the maximum supported value.");
            }

            state.AddExperience(amount);

            ProcessLevelUps();

            MetaProgressionChanged?.Invoke();
        }

        public bool TrySpendSkillPoints(int amount)
        {
            if (amount < 0)
                return false;

            if (state == null)
                return false;

            bool spent = state.TrySpendSkillPoints(amount);

            if (spent && amount > 0)
                MetaProgressionChanged?.Invoke();

            return spent;
        }

        public void RestoreProgression(
            int experience,
            int metaLevel,
            int skillPoints)
        {
            EnsureInitialized();

            if (!IsValidProgression(
                    experience,
                    metaLevel,
                    skillPoints))
            {
                throw new ArgumentException(
                    "The supplied meta progression values are invalid.");
            }

            state.RestoreProgression(
                experience,
                metaLevel,
                skillPoints);

            MetaProgressionChanged?.Invoke();
        }

        public static bool IsValidProgression(
            int experience,
            int metaLevel,
            int skillPoints)
        {
            if (experience < 0 ||
                metaLevel < 1 ||
                skillPoints < 0)
            {
                return false;
            }

            int experienceRequired =
                CalculateExperienceRequiredForLevel(metaLevel);

            return experience < experienceRequired;
        }

        public void ResetProgression()
        {
            EnsureInitialized();

            state.Clear();

            MetaProgressionChanged?.Invoke();
        }

        private void ProcessLevelUps()
        {
            while (
                state.Experience >=
                CalculateExperienceRequiredForNextLevel())
            {
                int experienceRequired =
                    CalculateExperienceRequiredForNextLevel();

                state.RemoveExperience(
                    experienceRequired);

                state.IncreaseMetaLevel();

                state.AddSkillPoint();
            }
        }

        private int CalculateExperienceRequiredForNextLevel()
        {
            return CalculateExperienceRequiredForLevel(
                state.MetaLevel);
        }

        private static int CalculateExperienceRequiredForLevel(
            int metaLevel)
        {
            double requiredExperience =
                BaseExperience *
                Math.Pow(
                    ExperienceGrowthMultiplier,
                    metaLevel - 1);

            if (requiredExperience >= int.MaxValue)
                return int.MaxValue;

            return Math.Max(
                1,
                (int)Math.Round(requiredExperience));
        }

        private void EnsureInitialized()
        {
            if (state == null)
            {
                throw new InvalidOperationException(
                    "PersistentMetaState is not initialized.");
            }
        }
    }
}
