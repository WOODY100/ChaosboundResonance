using System.Collections.Generic;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveCurrencyLoadPlan
    {
        private readonly List<SaveCurrencyAmountData> currencies;

        public IReadOnlyList<SaveCurrencyAmountData> Currencies =>
            currencies;

        public SaveCurrencyLoadPlan(
            List<SaveCurrencyAmountData> currencies)
        {
            this.currencies = currencies;
        }
    }
}