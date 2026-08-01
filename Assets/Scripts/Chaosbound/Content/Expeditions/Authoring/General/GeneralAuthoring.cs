using Chaosbound.Content.Expeditions.Enums;
using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.General
{
    [Serializable]
    public sealed class GeneralAuthoring
    {
        [SerializeField]
        private CompletionCondition m_CompletionCondition;

        [SerializeField]
        private DifficultyTier m_BaseDifficulty;

        public CompletionCondition CompletionCondition
        {
            get { return m_CompletionCondition; }
        }

        public DifficultyTier BaseDifficulty
        {
            get { return m_BaseDifficulty; }
        }
    }
}