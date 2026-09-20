using System.Collections.Generic;
using Chaosbound.Gameplay.Items.Runtime;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveInventoryLoadPlan
    {
        public List<ItemInstance> Items =
            new List<ItemInstance>();

        public Dictionary<string, int> Materials =
            new Dictionary<string, int>();

        public HashSet<string> SeenItemInstanceIds =
            new HashSet<string>();

        public HashSet<string> SeenMaterialIds =
            new HashSet<string>();
    }
}