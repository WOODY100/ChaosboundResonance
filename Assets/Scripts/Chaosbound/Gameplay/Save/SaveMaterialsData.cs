using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Save
{
    [Serializable]
    public sealed class SaveMaterialsData
    {
        public List<SaveMaterialAmountData> Materials =
            new List<SaveMaterialAmountData>();
    }
}