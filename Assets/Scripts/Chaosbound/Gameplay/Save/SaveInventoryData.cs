using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Save
{
    [Serializable]
    public sealed class SaveInventoryData
    {
        public List<SaveItemInstanceData> Items =
            new List<SaveItemInstanceData>();

        public SaveMaterialsData Materials;

        public SaveSeenIdsData ItemSeenState;

        public SaveSeenIdsData MaterialSeenState;
    }
}