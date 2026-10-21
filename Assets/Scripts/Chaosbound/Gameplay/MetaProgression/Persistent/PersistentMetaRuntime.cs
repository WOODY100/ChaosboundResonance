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

            return state.TrySpendSkillPoints(amount);
        }

        public void ResetProgression()
        {
            if (state == null)
            {
                throw new InvalidOperationException(
                    "PersistentMetaState is not initialized.");
            }

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
            double requiredExperience =
                BaseExperience *
                Math.Pow(
                    ExperienceGrowthMultiplier,
                    state.MetaLevel - 1);

            if (requiredExperience >= int.MaxValue)
            {
                return int.MaxValue;
            }

            return Math.Max(
                1,
                (int)Math.Round(
                    requiredExperience));
        }
    }
}