using Chaosbound.Gameplay.Threat.ValueObjects;

namespace Chaosbound.Gameplay.Threat.Runtime
{
    /// <summary>
    /// Represents the runtime threat economy of an expedition.
    /// Tracks the current threat capacity and the amount of threat
    /// currently occupied by alive enemies.
    ///
    /// The ThreatBudget is responsible only for managing the combat
    /// economy. It never spawns enemies, removes enemies or makes
    /// gameplay decisions.
    /// </summary>
    public sealed class ThreatBudget
    {
        /// <summary>
        /// Maximum threat capacity currently available.
        /// </summary>
        private ThreatCapacity m_CurrentCapacity;

        /// <summary>
        /// Total threat currently occupied by alive enemies.
        /// </summary>
        private float m_UsedThreat;

        /// <summary>
        /// Gets the current maximum threat capacity.
        /// </summary>
        public ThreatCapacity Capacity => m_CurrentCapacity;

        /// <summary>
        /// Gets the amount of threat currently occupied.
        /// </summary>
        public float UsedThreat => m_UsedThreat;

        /// <summary>
        /// Gets the currently available threat.
        ///
        /// This value can become negative when the current threat
        /// investment exceeds the available capacity.
        /// A negative value is considered a valid runtime state.
        /// </summary>
        public float AvailableThreat =>
            m_CurrentCapacity.Value - m_UsedThreat;

        /// <summary>
        /// Gets whether the current threat investment exceeds
        /// the available threat capacity.
        /// </summary>
        public bool IsOverBudget =>
            m_UsedThreat > m_CurrentCapacity.Value;

        /// <summary>
        /// Creates a new runtime threat budget.
        /// </summary>
        /// <param name="capacity">
        /// Initial threat capacity for the expedition.
        /// </param>
        public ThreatBudget(
            ThreatCapacity capacity)
        {
            m_CurrentCapacity = capacity;
            m_UsedThreat = 0f;
        }

        /// <summary>
        /// Updates the current threat capacity.
        ///
        /// This operation never modifies the occupied threat.
        /// If the new capacity becomes smaller than the currently
        /// occupied threat, the budget enters an over-budget state.
        /// </summary>
        /// <param name="capacity">
        /// New threat capacity.
        /// </param>
        public void UpdateCapacity(
            ThreatCapacity capacity)
        {
            m_CurrentCapacity = capacity;
        }

        /// <summary>
        /// Determines whether enough free capacity exists to allocate
        /// the specified threat cost.
        /// </summary>
        /// <param name="cost">
        /// Threat cost to evaluate.
        /// </param>
        /// <returns>
        /// True if enough capacity is available; otherwise false.
        /// </returns>
        public bool CanAllocate(
            ThreatCost cost)
        {
            return
                m_UsedThreat + cost.Value
                <=
                m_CurrentCapacity.Value;
        }

        /// <summary>
        /// Attempts to occupy the specified amount of threat.
        /// </summary>
        /// <param name="cost">
        /// Threat cost to occupy.
        /// </param>
        /// <returns>
        /// True if the allocation succeeded; otherwise false.
        /// </returns>
        public bool Occupy(
            ThreatCost cost)
        {
            if (!CanAllocate(cost))
                return false;

            m_UsedThreat += cost.Value;

            return true;
        }

        /// <summary>
        /// Releases previously occupied threat.
        ///
        /// Used threat is never allowed to become negative.
        /// </summary>
        /// <param name="cost">
        /// Threat cost to release.
        /// </param>
        public void Release(
            ThreatCost cost)
        {
            m_UsedThreat -= cost.Value;

            if (m_UsedThreat < 0f)
                m_UsedThreat = 0f;
        }
    }
}