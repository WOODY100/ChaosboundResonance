using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Currency.Runtime
{
    /// <summary>
    /// Stores currency accumulated during the current expedition.
    /// </summary>
    public sealed class ExpeditionCurrencyState
    {
        private readonly Dictionary<string, int> amounts =
            new Dictionary<string, int>();

        public int UniqueCurrencyCount => amounts.Count;

        public bool IsEmpty => amounts.Count == 0;

        public void Add(string contentId, int amount)
        {
            ValidateContentId(contentId);

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Currency amount must be greater than zero.");

            int currentAmount;

            if (amounts.TryGetValue(contentId, out currentAmount))
            {
                amounts[contentId] =
                    checked(currentAmount + amount);

                return;
            }

            amounts.Add(contentId, amount);
        }

        public int GetAmount(string contentId)
        {
            ValidateContentId(contentId);

            int amount;

            if (amounts.TryGetValue(contentId, out amount))
                return amount;

            return 0;
        }

        public bool Has(string contentId, int amount)
        {
            ValidateContentId(contentId);

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Required currency amount must be greater than zero.");

            return GetAmount(contentId) >= amount;
        }

        public IReadOnlyDictionary<string, int> GetAmounts()
        {
            return amounts;
        }

        public void Clear()
        {
            amounts.Clear();
        }

        private static void ValidateContentId(string contentId)
        {
            if (string.IsNullOrEmpty(contentId))
                throw new ArgumentException(
                    "Currency ContentId cannot be null or empty.",
                    nameof(contentId));
        }
    }
}