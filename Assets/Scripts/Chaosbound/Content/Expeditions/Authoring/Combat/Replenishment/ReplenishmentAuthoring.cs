using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Combat.Replenishment
{
    [Serializable]
    public sealed class ReplenishmentAuthoring
    {
        [Header("Recovery")]

        [SerializeField]
        private float m_InitialDelay = 1.5f;

        [SerializeField]
        private float m_RecoveryInterval = 0.75f;

        public float InitialDelay =>
            m_InitialDelay;

        public float RecoveryInterval =>
            m_RecoveryInterval;
    }
}