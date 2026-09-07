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

        /// <summary>
        /// Captures a snapshot of the XP fragments that are
        /// currently active in the expedition.
        /// </summary>
        public IReadOnlyList<ResonanceFragmentPickup>
            CaptureActiveFragments()
        {
            List<ResonanceFragmentPickup> snapshot =
                new List<ResonanceFragmentPickup>(
                    activeFragments.Count);

            for (int i = 0; i < activeFragments.Count; i++)
            {
                ResonanceFragmentPickup fragment =
                    activeFragments[i];

                if (fragment == null)
                    continue;

                if (!fragment.gameObject.activeInHierarchy)
                    continue;

                snapshot.Add(fragment);
            }

            return snapshot;
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