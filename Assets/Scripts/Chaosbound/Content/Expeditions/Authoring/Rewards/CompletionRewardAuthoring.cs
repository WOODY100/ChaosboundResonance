using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Rewards
{
    [Serializable]
    public sealed class CompletionRewardAuthoring
    {
        [Header("Item")]

        [SerializeField]
        private string m_itemContentId;

        [Header("Meta Progression")]

        [SerializeField]
        private int m_metaExperience;

        public string ItemContentId =>
            m_itemContentId;

        public int MetaExperience =>
            m_metaExperience;
    }
}