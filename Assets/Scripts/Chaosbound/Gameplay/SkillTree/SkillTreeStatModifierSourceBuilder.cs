using System;
using System.Collections.Generic;

public sealed class SkillTreeStatModifierSourceBuilder
{
    public const string SourceId = "SkillTree";

    public bool TryBuild(
        SkillTreeDatabase database,
        SkillTreeRuntimeState state,
        out ModifierSource source)
    {
        source = null;

        if (database == null || state == null)
            return false;

        ModifierSource result =
            new ModifierSource(SourceId);

        // Fail rather than silently retaining effects from a node
        // that no longer exists in the active database.
        foreach (string purchasedNodeId in state.PurchasedNodeIds)
        {
            if (!database.TryGetNode(
                purchasedNodeId,
                out SkillTreeNodeDefinition purchasedNode))
            {
                return false;
            }

            if (purchasedNode == null)
                return false;
        }

        IReadOnlyList<SkillTreeNodeDefinition> nodes =
            database.Nodes;

        if (nodes == null)
            return false;

        // Iterate in database order so the resulting source is deterministic.
        for (int i = 0; i < nodes.Count; i++)
        {
            SkillTreeNodeDefinition node = nodes[i];

            if (node == null ||
                string.IsNullOrWhiteSpace(node.NodeId) ||
                !state.IsNodePurchased(node.NodeId))
            {
                continue;
            }

            if (node.NodeType == Chaosbound.SkillTree.SkillTreeNodeType.Root)
                continue;

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
                        ChoiceOption option =
                            node.ChoiceOptions[optionIndex];

                        if (option == null ||
                            option.OptionId != selectedOptionId)
                        {
                            continue;
                        }

                        selectedOption = option;
                        break;
                    }
                }

                if (selectedOption == null)
                    return false;

                if (!TryAppendPlayerStatEffects(
                    selectedOption.Effects,
                    result))
                {
                    return false;
                }
            }
            else
            {
                if (!TryAppendPlayerStatEffects(
                    node.Effects,
                    result))
                {
                    return false;
                }
            }
        }

        source = result;
        return true;
    }

    private bool TryAppendPlayerStatEffects(
        SkillTreeEffect[] effects,
        ModifierSource source)
    {
        if (effects == null || effects.Length == 0)
            return true;

        for (int i = 0; i < effects.Length; i++)
        {
            SkillTreeEffect effect = effects[i];

            if (effect == null)
                return false;

            PlayerStatEffect statEffect =
                effect as PlayerStatEffect;

            // Other effect families are intentionally handled by
            // their own integrations in later phases.
            if (statEffect == null)
                continue;

            if (!Enum.IsDefined(
                    typeof(StatType),
                    statEffect.StatType))
            {
                return false;
            }

            if (!Enum.IsDefined(
                    typeof(ModifierType),
                    statEffect.ModifierType))
            {
                return false;
            }

            if (float.IsNaN(statEffect.Value) ||
                float.IsInfinity(statEffect.Value))
            {
                return false;
            }

            source.Modifiers.Add(
                new StatModifier
                {
                    StatType = statEffect.StatType,
                    ModifierType = statEffect.ModifierType,
                    Value = statEffect.Value
                });
        }

        return true;
    }
}
