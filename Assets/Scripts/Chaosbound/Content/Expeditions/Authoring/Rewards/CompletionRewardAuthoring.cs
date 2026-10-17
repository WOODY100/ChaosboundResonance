using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Authoring.Rewards
{
    [System.Serializable]
    public sealed class CompletionRewardAuthoring
    {
        [Header("Items")]

        [SerializeField]
        private List<string> m_itemContentIds = new List<string>();

        [Header("Meta Progression")]

        [SerializeField]
        private int m_metaExperience;

        public IReadOnlyList<string> ItemContentIds =>
            m_itemContentIds;

        public int MetaExperience =>
            m_metaExperience;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_itemContentIds == null)
                m_itemContentIds = new List<string>();

            m_metaExperience = Mathf.Max(0, m_metaExperience);
        }
#endif
    }
}