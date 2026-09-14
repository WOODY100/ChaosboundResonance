using System.Collections.Generic;

namespace Chaosbound.Gameplay.Inventory.Persistent
{
    public sealed class PersistentMaterialsState
    {
        private readonly Dictionary<string, int> amounts =
            new Dictionary<string, int>();

        public bool Add(
            string contentId,
            int amount)
        {
            if (string.IsNullOrWhiteSpace(contentId))
                return false;

            if (amount < 0)
                return false;

            contentId = contentId.Trim();

            if (amounts.TryGetValue(
                    contentId,
                    out int currentAmount))
            {
                amounts[contentId] =
                    currentAmount + amount;
            }
            else
            {
                amounts.Add(
                    contentId,
                    amount);
            }

            return true;
        }

        public bool Remove(
            string contentId,
            int amount)
        {
            if (string.IsNullOrWhiteSpace(contentId))
                return false;

            if (amount < 0)
                return false;

            contentId = contentId.Trim();

            if (!amounts.TryGetValue(
                    contentId,
                    out int currentAmount))
            {
                return false;
            }

            if (currentAmount < amount)
                return false;

            amounts[contentId] =
                currentAmount - amount;

            return true;
        }

        public bool Has(
            string contentId,
            int amount)
        {
            if (string.IsNullOrWhiteSpace(contentId))
                return false;

            if (amount < 0)
                return false;

            contentId = contentId.Trim();

            return amounts.TryGetValue(
                       contentId,
                       out int currentAmount)
                   && currentAmount >= amount;
        }

        public int GetAmount(
            string contentId)
        {
            if (string.IsNullOrWhiteSpace(contentId))
                return 0;

            contentId = contentId.Trim();

            return amounts.TryGetValue(
                       contentId,
                       out int amount)
                ? amount
                : 0;
        }

        public IReadOnlyDictionary<string, int>
            GetAmounts()
        {
            return amounts;
        }

        public void Clear()
        {
            amounts.Clear();
        }
    }
}