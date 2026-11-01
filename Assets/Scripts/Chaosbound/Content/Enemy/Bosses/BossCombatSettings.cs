using System;
using UnityEngine;

namespace Chaosbound.Content.Enemy.Bosses
{
    [Serializable]
    public struct BossCombatSettings
    {
        [SerializeField, Min(0.01f)]
        private float m_WaitReevaluationInterval;

        public float WaitReevaluationInterval =>
            m_WaitReevaluationInterval;

        public bool IsValid =>
            m_WaitReevaluationInterval > 0f;
    }
}
