using Chaosbound.Gameplay.EnemySolver.Enums;
using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Enemy.TacticalIdentity
{
    /// <summary>
    /// Configurable tactical affinity used by an expedition.
    /// Describes a tactical capability favored during enemy evaluation.
    /// </summary>
    [Serializable]
    public sealed class CapabilityAffinityAuthoring
    {
        [SerializeField]
        private TacticalCapability m_Capability;

        public TacticalCapability Capability =>
            m_Capability;

        [SerializeField]
        [Min(0f)]
        private float m_BonusScore = 50f;

        public float BonusScore =>
            m_BonusScore;
    }
}