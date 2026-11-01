using System;

namespace Chaosbound.Gameplay.Save
{
    public sealed class SaveSkillTreeLoadPlan
    {
        public string TreeId { get; }
        public string[] PurchasedNodeIds { get; }
        public SaveSkillTreeChoiceData[] SelectedChoices { get; }

        public SaveSkillTreeLoadPlan(
            string treeId,
            string[] purchasedNodeIds,
            SaveSkillTreeChoiceData[] selectedChoices)
        {
            if (string.IsNullOrWhiteSpace(treeId))
                throw new ArgumentException(
                    "TreeId cannot be empty.",
                    nameof(treeId));

            if (purchasedNodeIds == null)
                throw new ArgumentNullException(nameof(purchasedNodeIds));

            if (selectedChoices == null)
                throw new ArgumentNullException(nameof(selectedChoices));

            TreeId = treeId;
            PurchasedNodeIds = (string[])purchasedNodeIds.Clone();
            SelectedChoices =
                (SaveSkillTreeChoiceData[])selectedChoices.Clone();
        }
    }
}
