using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Combat
{
    [Serializable]
    public sealed class CombatTypeCompositionAuthoring
    {
        [Header("Combat Type Percentage")]

        [SerializeField]
        [Range(0f, 1f)]
        private float m_Percentage = 0.5f;

        [Header("Role Composition")]

        [SerializeField]
        [Range(0f, 1f)]
        private float m_NormalPercentage = 0.70f;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_RunnerPercentage = 0.20f;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_TankPercentage = 0.10f;

        public float Percentage =>
            m_Percentage;

        public float NormalPercentage =>
            m_NormalPercentage;

        public float RunnerPercentage =>
            m_RunnerPercentage;

        public float TankPercentage =>
            m_TankPercentage;
    }
}