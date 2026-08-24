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
        private CombatTypeCompositionAuthoring m_Melee =
            new();

        [SerializeField]
        private CombatTypeCompositionAuthoring m_Ranged =
            new();

        public CombatTypeCompositionAuthoring Melee =>
            m_Melee;

        public CombatTypeCompositionAuthoring Ranged =>
            m_Ranged;

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