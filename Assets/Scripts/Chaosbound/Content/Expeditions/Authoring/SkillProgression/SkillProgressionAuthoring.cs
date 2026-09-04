using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.SkillProgression
{
    [Serializable]
    public sealed class SkillProgressionAuthoring
    {
        [Header("Skill Level")]

        [SerializeField]
        private int m_maxSkillLevel = 20;

        [Header("Evolution")]

        [SerializeField]
        private int m_evolutionRequiredLevel = 10;

        public int MaxSkillLevel =>
            m_maxSkillLevel;

        public int EvolutionRequiredLevel =>
            m_evolutionRequiredLevel;
    }
}