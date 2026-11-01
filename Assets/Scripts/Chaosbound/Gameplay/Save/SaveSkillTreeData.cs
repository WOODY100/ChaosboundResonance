using System;

namespace Chaosbound.Gameplay.Save
{
    [Serializable]
    public sealed class SaveSkillTreeData
    {
        public string TreeId;
        public string[] PurchasedNodeIds;
        public SaveSkillTreeChoiceData[] SelectedChoices;
    }
}
