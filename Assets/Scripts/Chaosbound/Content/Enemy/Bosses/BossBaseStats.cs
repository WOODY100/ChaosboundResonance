using System;
using UnityEngine;

namespace Chaosbound.Content.Enemy.Bosses
{
    [Serializable]
    public struct BossBaseStats
    {
        [SerializeField, Min(1f)] private float m_MaxHP;
        [SerializeField, Min(0.01f)] private float m_BaseDamage;

        public float MaxHP => m_MaxHP;
        public float BaseDamage => m_BaseDamage;

        public bool IsValid =>
            m_MaxHP > 0f &&
            m_BaseDamage > 0f;
    }
}
