using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Combat.SpawnPattern
{
    [Serializable]
    public sealed class SpawnPatternAuthoring
    {
        [Header("Pattern Weights")]

        [SerializeField]
        [Range(0f, 1f)]
        private float m_PerimeterPercentage = 1f;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_FrontPercentage = 0f;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_RearPercentage = 0f;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_FlankPercentage = 0f;

        public float PerimeterPercentage =>
            m_PerimeterPercentage;

        public float FrontPercentage =>
            m_FrontPercentage;

        public float RearPercentage =>
            m_RearPercentage;

        public float FlankPercentage =>
            m_FlankPercentage;
    }
}