using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Save
{
    [Serializable]
    public sealed class ExpeditionProgressData
    {
        public List<string> CompletedExpeditionIds =
            new List<string>();
    }
}
