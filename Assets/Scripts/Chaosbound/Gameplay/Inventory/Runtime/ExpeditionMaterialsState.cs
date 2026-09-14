using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Inventory.Runtime
{
    public sealed class ExpeditionMaterialsState
    {
        private readonly Dictionary<string, int> amounts =
            new Dictionary<string, int>();

        public int UniqueMaterialCount => amounts.Count;

        public bool IsEmpty => amounts.Count == 0;

        public void Add(string materialId, int amount)
        {
            ValidateMaterialId(materialId);

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Material amount must be greater than zero.");

            int currentAmount;

            if (amounts.TryGetValue(materialId, out currentAmount))
            {
                amounts[materialId] = checked(currentAmount + amount);
                return;
            }

            amounts.Add(materialId, amount);
        }

        public int GetAmount(string materialId)
        {
            ValidateMaterialId(materialId);

            int amount;

            if (amounts.TryGetValue(materialId, out amount))
                return amount;

            return 0;
        }

        public bool Has(string materialId, int amount)
        {
            ValidateMaterialId(materialId);

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Required material amount must be greater than zero.");

            return GetAmount(materialId) >= amount;
        }

        public bool Remove(string materialId, int amount)
        {
            ValidateMaterialId(materialId);

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    amount,
                    "Material amount must be greater than zero.");

            int currentAmount;

            if (!amounts.TryGetValue(materialId, out currentAmount))
                return false;

            if (currentAmount < amount)
                return false;

            amounts[materialId] = currentAmount - amount;

            return true;
        }

        public IReadOnlyDictionary<string, int> GetAmounts()
        {
            return amounts;
        }

        private static void ValidateMaterialId(string materialId)
        {
            if (string.IsNullOrEmpty(materialId))
                throw new ArgumentException(
                    "Material ID cannot be null or empty.",
                    nameof(materialId));
        }
    }
}