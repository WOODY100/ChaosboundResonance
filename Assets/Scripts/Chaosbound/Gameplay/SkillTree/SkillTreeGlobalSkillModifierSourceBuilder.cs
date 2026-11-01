using System;
using System.Collections.Generic;

public sealed class SkillTreeGlobalSkillModifierSourceBuilder
{
    public bool TryBuild(
        SkillTreeDatabase database,
        SkillTreeRuntimeState state,
        out List<ModifierEntry> modifiers)
    {
        modifiers = null;

        if (database == null || state == null)
            return false;

        // Validate that every purchased node still exists in the active database.
        foreach (string purchasedNodeId in state.PurchasedNodeIds)
        {
            if (!database.TryGetNode(
                purchasedNodeId,
                out SkillTreeNodeDefinition purchasedNode) ||
                purchasedNode == null)
            {
                return false;
            }
        }

        IReadOnlyList<SkillTreeNodeDefinition> nodes = database.Nodes;
        if (nodes == null)
            return false;

        List<ModifierEntry> result = new List<ModifierEntry>();

        // Database order provides deterministic output.
        for (int i = 0; i < nodes.Count; i++)
        {
            SkillTreeNodeDefinition node = nodes[i];

            if (node == null ||
                string.IsNullOrWhiteSpace(node.NodeId) ||
                !state.IsNodePurchased(node.NodeId) ||
                node.NodeType == Chaosbound.SkillTree.SkillTreeNodeType.Root)
            {
                continue;
            }

            SkillTreeEffect[] effects;

            if (node.NodeType == Chaosbound.SkillTree.SkillTreeNodeType.Choice)
            {
                if (!state.TryGetSelectedChoice(
                    node.NodeId,
                    out string selectedOptionId))
                {
                    return false;
                }

                ChoiceOption selectedOption = null;
                if (node.ChoiceOptions != null)
                {
                    for (int optionIndex = 0;
                         optionIndex < node.ChoiceOptions.Length;
                         optionIndex++)
                    {
                        ChoiceOption option = node.ChoiceOptions[optionIndex];
                        if (option != null && option.OptionId == selectedOptionId)
                        {
                            selectedOption = option;
                            break;
                        }
                    }
                }

                if (selectedOption == null)
                    return false;

                effects = selectedOption.Effects;
            }
            else
            {
                effects = node.Effects;
            }

            if (!TryAppend(effects, result))
                return false;
        }

        modifiers = result;
        return true;
    }

    private bool TryAppend(
        SkillTreeEffect[] effects,
        List<ModifierEntry> result)
    {
        if (effects == null || effects.Length == 0)
            return true;

        for (int i = 0; i < effects.Length; i++)
        {
            SkillTreeEffect effect = effects[i];
            if (effect == null)
                return false;

            GlobalSkillModifierEffect globalEffect =
                effect as GlobalSkillModifierEffect;

            // Other effect families are owned by their respective integrations.
            if (globalEffect == null)
                continue;

            if (!Enum.IsDefined(
                typeof(SkillModifierType),
                globalEffect.ModifierType))
            {
                return false;
            }

            if (float.IsNaN(globalEffect.Value) ||
                float.IsInfinity(globalEffect.Value))
            {
                return false;
            }

            result.Add(new ModifierEntry
            {
                Type = globalEffect.ModifierType,
                Value = globalEffect.Value
            });
        }

        return true;
    }
}
