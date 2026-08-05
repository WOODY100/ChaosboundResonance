using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Enemy.TacticalIdentity
{
    /// <summary>
    /// Configurable tactical identity for an expedition.
    /// Describes the tactical affinities favored during enemy evaluation.
    /// </summary>
    [Serializable]
    public sealed class TacticalIdentityAuthoring
    {
        [SerializeField]
        private List<CapabilityAffinityAuthoring> m_Affinities = new();

        /// <summary>
        /// Gets every configured tactical affinity.
        /// </summary>
        public IReadOnlyList<CapabilityAffinityAuthoring> Affinities =>
            m_Affinities;
    }
}