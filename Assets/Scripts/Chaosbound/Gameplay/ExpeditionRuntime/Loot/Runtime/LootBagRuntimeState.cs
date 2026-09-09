using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Loot.Runtime
{
    /// <summary>
    /// Tracks Loot Bags materialized during
    /// the current expedition.
    /// </summary>
    public sealed class LootBagRuntimeState
    {
        private readonly List<LootBag>
            activeLootBags =
                new List<LootBag>();

        public void Register(
            LootBag lootBag)
        {
            if (lootBag == null)
                return;

            if (activeLootBags.Contains(lootBag))
                return;

            activeLootBags.Add(lootBag);
        }

        public void Unregister(
            LootBag lootBag)
        {
            if (lootBag == null)
                return;

            activeLootBags.Remove(lootBag);
        }

        public void Cleanup()
        {
            if (activeLootBags.Count == 0)
                return;

            LootBag[] snapshot =
                activeLootBags.ToArray();

            activeLootBags.Clear();

            for (int i = 0; i < snapshot.Length; i++)
            {
                LootBag lootBag =
                    snapshot[i];

                if (lootBag == null)
                    continue;

                if (!lootBag.gameObject.activeInHierarchy)
                    continue;

                lootBag.ReturnToPool();
            }
        }
    }
}
