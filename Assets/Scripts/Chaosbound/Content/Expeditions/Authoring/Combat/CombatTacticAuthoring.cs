using Chaosbound.Content.Expeditions.Authoring.Combat.SpawnPattern;
using Chaosbound.Content.Expeditions.Authoring.Combat.Replenishment;
using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Combat
{
    [Serializable]
    public sealed class CombatTacticAuthoring
    {
        [Header("MaximumTarget")]

        [SerializeField]
        private int m_MaximumTarget = 10;

        public int MaximumTarget =>
            m_MaximumTarget;

        [Header("Composition")]

        [SerializeField]
        private float m_NormalPercentage = 0.70f;

        [SerializeField]
        private float m_RunnerPercentage = 0.20f;

        [SerializeField]
        private float m_TankPercentage = 0.10f;

        public float NormalPercentage =>
            m_NormalPercentage;

        public float RunnerPercentage =>
            m_RunnerPercentage;

        public float TankPercentage =>
            m_TankPercentage;

        [Header("Replenishment")]

        [SerializeField]
        private ReplenishmentAuthoring m_Replenishment =
            new();

        public ReplenishmentAuthoring Replenishment =>
            m_Replenishment;

        [Header("Spawn Pattern")]

        [SerializeField]
        private SpawnPatternAuthoring m_SpawnPattern =
            new();

        public SpawnPatternAuthoring SpawnPattern =>
            m_SpawnPattern;
    }
}