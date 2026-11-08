using Chaosbound.Content.Materials;
using System;
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
                try
                {
                    amounts[contentId] =
                        checked(currentAmount + amount);
                }
                catch (OverflowException)
                {
                    return false;
                }
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

            if (amount <= 0)
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

            // Never remove the material entry at zero.
            return true;
        }

        public bool Has(
            string contentId,
            int amount)
        {
            if (string.IsNullOrWhiteSpace(contentId))
                return false;

            if (amount <= 0)
                return false;

            contentId = contentId.Trim();

            return amounts.TryGetValue(
                       contentId,
                       out int currentAmount)
                   && currentAmount >= amount;
        }

        public bool TrySpend(
            IReadOnlyList<MaterialCost> costs)
        {
            if (costs == null || costs.Count == 0)
                return false;

            HashSet<string> validatedIds =
                new HashSet<string>();

            // Validate the complete transaction first.
            for (int i = 0; i < costs.Count; i++)
            {
                MaterialCost cost = costs[i];

                if (!cost.IsValid)
                    return false;

                string contentId = cost.ContentId.Trim();

                if (!validatedIds.Add(contentId))
                    return false;

                if (!Has(contentId, cost.Amount))
                    return false;
            }

            // Only spend after every cost has passed validation.
            for (int i = 0; i < costs.Count; i++)
            {
                MaterialCost cost = costs[i];

                Remove(
                    cost.ContentId,
                    cost.Amount);
            }

            return true;
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