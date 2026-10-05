using System;

namespace Chaosbound.Gameplay.MetaProgression.Persistent
{
    public sealed class PersistentMetaState
    {
        private int experience;

        private int metaLevel = 1;

        private int skillPoints;

        public int Experience =>
            experience;

        public int MetaLevel =>
            metaLevel;

        public int SkillPoints =>
            skillPoints;

        public void AddExperience(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Meta experience cannot be negative.");

            experience = checked(experience + amount);
        }

        public bool CanAddExperience(int amount)
        {
            if (amount < 0)
                return false;

            return amount <=
                   int.MaxValue - experience;
        }

        public void RemoveExperience(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Meta experience cannot be negative.");

            if (amount > experience)
                throw new InvalidOperationException(
                    "Cannot remove more meta experience " +
                    "than the current amount.");

            experience -= amount;
        }

        public void IncreaseMetaLevel()
        {
            metaLevel = checked(metaLevel + 1);
        }

        public void AddSkillPoint()
        {
            skillPoints = checked(skillPoints + 1);
        }

        public bool TrySpendSkillPoints(int amount)
        {
            if (amount < 0)
                return false;

            if (amount > skillPoints)
                return false;

            skillPoints -= amount;

            return true;
        }

        public void Clear()
        {
            experience = 0;
            metaLevel = 1;
            skillPoints = 0;
        }
    }
}