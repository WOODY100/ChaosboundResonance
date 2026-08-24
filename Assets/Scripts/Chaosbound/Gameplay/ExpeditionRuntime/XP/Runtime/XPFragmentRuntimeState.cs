using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.XP.Runtime
{
    /// <summary>
    /// Tracks XP fragments materialized during
    /// the current expedition.
    /// </summary>
    public sealed class XPFragmentRuntimeState
    {
        private readonly List<ResonanceFragmentPickup>
            activeFragments =
                new List<ResonanceFragmentPickup>();

        public void Register(
            ResonanceFragmentPickup fragment)
        {
            if (fragment == null)
                return;

            if (activeFragments.Contains(fragment))
                return;

            activeFragments.Add(fragment);
        }

        public void Cleanup()
        {
            for (int i = activeFragments.Count - 1; i >= 0; i--)
            {
                ResonanceFragmentPickup fragment =
                    activeFragments[i];

                if (fragment == null)
                {
                    activeFragments.RemoveAt(i);
                    continue;
                }

                if (!fragment.gameObject.activeInHierarchy)
                {
                    activeFragments.RemoveAt(i);
                    continue;
                }

                fragment.Cleanup();

                activeFragments.RemoveAt(i);
            }

            activeFragments.Clear();
        }
    }
}