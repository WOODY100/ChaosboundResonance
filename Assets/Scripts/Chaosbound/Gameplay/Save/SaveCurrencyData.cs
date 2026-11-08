using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Save
{
    [Serializable]
    public sealed class SaveCurrencyData
    {
        public List<SaveCurrencyAmountData> Currencies =
            new List<SaveCurrencyAmountData>();
    }

    [Serializable]
    public sealed class SaveCurrencyAmountData
    {
        public string ContentId;
        public int Amount;
    }
}