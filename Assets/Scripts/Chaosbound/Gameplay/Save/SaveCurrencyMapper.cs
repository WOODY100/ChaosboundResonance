using System;
using System.Collections.Generic;
using Chaosbound.Gameplay.Currency.Persistent;

namespace Chaosbound.Gameplay.Save
{
    public static class SaveCurrencyMapper
    {
        public static SaveCurrencyData ToSaveData(
            PersistentCurrencyState state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));

            SaveCurrencyData data =
                new SaveCurrencyData();

            IReadOnlyDictionary<string, int> amounts =
                state.GetAmounts();

            foreach (KeyValuePair<string, int> entry in amounts)
            {
                data.Currencies.Add(
                    new SaveCurrencyAmountData
                    {
                        ContentId = entry.Key,
                        Amount = entry.Value
                    });
            }

            return data;
        }

        public static bool TryPrepareLoad(
            SaveCurrencyData data,
            out SaveCurrencyLoadPlan loadPlan)
        {
            loadPlan = null;

            if (data == null || data.Currencies == null)
                return false;

            HashSet<string> registeredIds =
                new HashSet<string>(StringComparer.Ordinal);

            List<SaveCurrencyAmountData> currencies =
                new List<SaveCurrencyAmountData>();

            for (int i = 0; i < data.Currencies.Count; i++)
            {
                SaveCurrencyAmountData entry =
                    data.Currencies[i];

                if (entry == null)
                    return false;

                if (string.IsNullOrWhiteSpace(entry.ContentId))
                    return false;

                if (entry.Amount < 0)
                    return false;

                string contentId = entry.ContentId.Trim();

                if (!registeredIds.Add(contentId))
                    return false;

                currencies.Add(
                    new SaveCurrencyAmountData
                    {
                        ContentId = contentId,
                        Amount = entry.Amount
                    });
            }

            loadPlan =
                new SaveCurrencyLoadPlan(currencies);

            return true;
        }

        public static void ApplyLoadPlan(
            SaveCurrencyLoadPlan loadPlan,
            PersistentCurrencyState state)
        {
            if (loadPlan == null)
                throw new ArgumentNullException(nameof(loadPlan));

            if (state == null)
                throw new ArgumentNullException(nameof(state));

            state.Clear();

            for (int i = 0; i < loadPlan.Currencies.Count; i++)
            {
                SaveCurrencyAmountData entry =
                    loadPlan.Currencies[i];

                if (!state.Add(
                        entry.ContentId,
                        entry.Amount))
                {
                    throw new InvalidOperationException(
                        "Failed to restore currency amount: " +
                        entry.ContentId);
                }
            }
        }
    }
}