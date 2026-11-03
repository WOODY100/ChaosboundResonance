using System;
using UnityEngine;

namespace Chaosbound.Content.Enemy.Bosses
{
    [Serializable]
    public struct BossMovementSettings
    {
        [SerializeField, Min(0.01f)] private float m_PreferredDistance;
        [SerializeField, Min(0f)] private float m_MinimumDistance;
        [SerializeField, Min(0.01f)] private float m_MaximumRetreatDistance;
        [SerializeField, Min(0f)] private float m_RetreatCooldown;
        [SerializeField, Min(0.01f)] private float m_RetreatSpeed;
        [SerializeField, Min(0.01f)] private float m_RotationSpeed;
        [SerializeField, Min(0f)] private float m_StoppingDistance;
        [SerializeField, Min(0.1f)] private float m_NavigationNoProgressTimeout;
        [SerializeField, Min(0.1f)] private float m_NavigationRecoveryTimeout;
        [SerializeField, Min(0f)] private int m_NavigationRecoveryAttempts;
        [SerializeField, Min(0.01f)] private float m_NavigationProgressThreshold;

        public float PreferredDistance => m_PreferredDistance;
        public float MinimumDistance => m_MinimumDistance;
        public float MaximumRetreatDistance => m_MaximumRetreatDistance;
        public float RetreatCooldown => m_RetreatCooldown;
        public float RetreatSpeed => m_RetreatSpeed;
        public float RotationSpeed => m_RotationSpeed;
        public float StoppingDistance => m_StoppingDistance;
        public float NavigationNoProgressTimeout =>
            m_NavigationNoProgressTimeout > 0f ? m_NavigationNoProgressTimeout : 1.5f;
        public float NavigationRecoveryTimeout =>
            m_NavigationRecoveryTimeout > 0f ? m_NavigationRecoveryTimeout : 1.5f;
        public int NavigationRecoveryAttempts =>
            m_NavigationRecoveryAttempts > 0 ? m_NavigationRecoveryAttempts : 1;
        public float NavigationProgressThreshold =>
            m_NavigationProgressThreshold > 0f ? m_NavigationProgressThreshold : 0.15f;

        public bool IsValid =>
            m_PreferredDistance > 0f &&
            m_MinimumDistance >= 0f &&
            m_MinimumDistance < m_PreferredDistance &&
            m_MaximumRetreatDistance > 0f &&
            m_RetreatCooldown >= 0f &&
            m_RetreatSpeed > 0f &&
            m_RotationSpeed > 0f &&
            m_StoppingDistance >= 0f;
    }
}
