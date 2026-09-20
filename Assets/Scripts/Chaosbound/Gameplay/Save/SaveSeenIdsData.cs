using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Save
{
    [Serializable]
    public sealed class SaveSeenIdsData
    {
        public List<string> Ids =
            new List<string>();
    }
}