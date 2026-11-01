using System;
using System.Collections.Generic;
using Chaosbound.SkillTree;

namespace Chaosbound.Gameplay.Save
{
    public static class SaveSkillTreeMapper
    {
        public static SaveSkillTreeData ToSaveData(
            SkillTreeDatabase database,
            SkillTreeRuntimeState state)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));

            if (state == null)
                throw new ArgumentNullException(nameof(state));

            if (string.IsNullOrWhiteSpace(database.TreeId))
                throw new InvalidOperationException(
                    "SkillTreeDatabase has an empty TreeId.");

            List<string> purchasedNodeIds =
                new List<string>(state.PurchasedNodeIds);
            purchasedNodeIds.Sort(StringComparer.Ordinal);

            List<SaveSkillTreeChoiceData> selectedChoices =
                new List<SaveSkillTreeChoiceData>();

            foreach (KeyValuePair<string, string> pair in state.SelectedChoices)
            {
                selectedChoices.Add(new SaveSkillTreeChoiceData
                {
                    NodeId = pair.Key,
                    OptionId = pair.Value
                });
            }

            selectedChoices.Sort(
                delegate(
                    SaveSkillTreeChoiceData left,
                    SaveSkillTreeChoiceData right)
                {
                    return StringComparer.Ordinal.Compare(
                        left.NodeId,
                        right.NodeId);
                });

            return new SaveSkillTreeData
            {
                TreeId = database.TreeId,
                PurchasedNodeIds = purchasedNodeIds.ToArray(),
                SelectedChoices = selectedChoices.ToArray()
            };
        }

        public static bool TryPrepareLoad(
            SaveSkillTreeData saveData,
            SkillTreeDatabase database,
            out SaveSkillTreeLoadPlan loadPlan)
        {
            loadPlan = null;

            if (saveData == null || database == null)
                return false;

            if (string.IsNullOrWhiteSpace(database.TreeId))
                return false;

            if (!string.Equals(
                saveData.TreeId,
                database.TreeId,
                StringComparison.Ordinal))
                return false;

            if (saveData.PurchasedNodeIds == null ||
                saveData.SelectedChoices == null)
                return false;

            HashSet<string> purchasedNodeIds =
                new HashSet<string>(StringComparer.Ordinal);

            for (int i = 0; i < saveData.PurchasedNodeIds.Length; i++)
            {
                string nodeId = saveData.PurchasedNodeIds[i];

                if (string.IsNullOrWhiteSpace(nodeId))
                    return false;

                if (!purchasedNodeIds.Add(nodeId))
                    return false;

                SkillTreeNodeDefinition node = database.GetNode(nodeId);

                if (node == null ||
                    node.NodeType == SkillTreeNodeType.Root)
                    return false;
            }

            Dictionary<string, string> selectedChoices =
                new Dictionary<string, string>(StringComparer.Ordinal);

            for (int i = 0; i < saveData.SelectedChoices.Length; i++)
            {
                SaveSkillTreeChoiceData choice = saveData.SelectedChoices[i];

                if (choice == null ||
                    string.IsNullOrWhiteSpace(choice.NodeId) ||
                    string.IsNullOrWhiteSpace(choice.OptionId))
                    return false;

                if (selectedChoices.ContainsKey(choice.NodeId))
                    return false;

                if (!purchasedNodeIds.Contains(choice.NodeId))
                    return false;

                SkillTreeNodeDefinition node =
                    database.GetNode(choice.NodeId);

                if (node == null ||
                    node.NodeType != SkillTreeNodeType.Choice ||
                    !ContainsChoiceOption(node, choice.OptionId))
                    return false;

                selectedChoices.Add(choice.NodeId, choice.OptionId);
            }

            foreach (string nodeId in purchasedNodeIds)
            {
                SkillTreeNodeDefinition node = database.GetNode(nodeId);

                if (node == null)
                    return false;

                if (!EvaluateRequirement(
                    node.Requirement,
                    purchasedNodeIds,
                    database))
                    return false;

                if (node.NodeType == SkillTreeNodeType.Choice &&
                    !selectedChoices.ContainsKey(nodeId))
                    return false;

                if (node.NodeType != SkillTreeNodeType.Choice &&
                    selectedChoices.ContainsKey(nodeId))
                    return false;
            }

            loadPlan = new SaveSkillTreeLoadPlan(
                saveData.TreeId,
                saveData.PurchasedNodeIds,
                saveData.SelectedChoices);

            return true;
        }

        public static bool ApplyLoadPlan(
            SaveSkillTreeLoadPlan loadPlan,
            SkillTreeRuntimeState state,
            SkillTreeDatabase database)
        {
            if (loadPlan == null || state == null || database == null)
                return false;

            if (!string.Equals(
                loadPlan.TreeId,
                database.TreeId,
                StringComparison.Ordinal))
                return false;

            HashSet<string> purchasedNodeIds =
                new HashSet<string>(
                    loadPlan.PurchasedNodeIds,
                    StringComparer.Ordinal);

            Dictionary<string, string> selectedChoices =
                new Dictionary<string, string>(StringComparer.Ordinal);

            for (int i = 0; i < loadPlan.SelectedChoices.Length; i++)
            {
                SaveSkillTreeChoiceData choice =
                    loadPlan.SelectedChoices[i];

                if (choice == null ||
                    selectedChoices.ContainsKey(choice.NodeId))
                    return false;

                selectedChoices.Add(choice.NodeId, choice.OptionId);
            }

            return state.Restore(purchasedNodeIds, selectedChoices);
        }

        private static bool ContainsChoiceOption(
            SkillTreeNodeDefinition node,
            string optionId)
        {
            if (node.ChoiceOptions == null)
                return false;

            for (int i = 0; i < node.ChoiceOptions.Length; i++)
            {
                ChoiceOption option = node.ChoiceOptions[i];

                if (option == null)
                    continue;

                if (string.Equals(
                    option.OptionId,
                    optionId,
                    StringComparison.Ordinal))
                    return true;
            }

            return false;
        }

        private static bool EvaluateRequirement(
            NodeRequirement requirement,
            HashSet<string> purchasedNodeIds,
            SkillTreeDatabase database)
        {
            if (requirement == null)
                return true;

            NodePurchasedRequirement purchased =
                requirement as NodePurchasedRequirement;

            if (purchased != null)
            {
                if (string.IsNullOrWhiteSpace(purchased.NodeId))
                    return false;

                SkillTreeNodeDefinition requiredNode =
                    database.GetNode(purchased.NodeId);

                if (requiredNode == null)
                    return false;

                return requiredNode.NodeType == SkillTreeNodeType.Root ||
                       purchasedNodeIds.Contains(purchased.NodeId);
            }

            AllRequirement all = requirement as AllRequirement;

            if (all != null)
            {
                if (all.Requirements == null ||
                    all.Requirements.Length == 0)
                    return false;

                for (int i = 0; i < all.Requirements.Length; i++)
                {
                    if (!EvaluateRequirement(
                        all.Requirements[i],
                        purchasedNodeIds,
                        database))
                        return false;
                }

                return true;
            }

            AnyRequirement any = requirement as AnyRequirement;

            if (any != null)
            {
                if (any.Requirements == null ||
                    any.Requirements.Length == 0)
                    return false;

                for (int i = 0; i < any.Requirements.Length; i++)
                {
                    if (EvaluateRequirement(
                        any.Requirements[i],
                        purchasedNodeIds,
                        database))
                        return true;
                }

                return false;
            }

            return false;
        }
    }
}
