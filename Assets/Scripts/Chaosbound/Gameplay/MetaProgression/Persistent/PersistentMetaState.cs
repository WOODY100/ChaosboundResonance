using System;

namespace Chaosbound.Gameplay.MetaProgression.Persistent
{
    public sealed class PersistentMetaState
    {
        private int experience;

        public int Experience =>
            experience;

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

        public void Clear()
        {
            experience = 0;
        }
    }
}