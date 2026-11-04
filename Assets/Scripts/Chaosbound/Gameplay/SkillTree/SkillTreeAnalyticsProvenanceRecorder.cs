#if UNITY_EDITOR
using Chaosbound.Development.Analytics;
using System.Collections.Generic;

/// <summary>
/// Editor-only observer that preserves the source identity of active Skill Tree
/// effects when their modifier sets are rebuilt. It does not alter gameplay data.
/// </summary>
public static class SkillTreeAnalyticsProvenanceRecorder
{
    public static void RegisterEffects(
        SkillTreeDatabase database,
        SkillTreeRuntimeState state,
        bool playerStats)
    {
        if (!ExpeditionTelemetryCollector.IsRecording ||
            database == null || state == null)
            return;

        IReadOnlyList<SkillTreeNodeDefinition> nodes = database.Nodes;
        if (nodes == null)
            return;

        for (int nodeIndex = 0; nodeIndex < nodes.Count; nodeIndex++)
        {
            SkillTreeNodeDefinition node = nodes[nodeIndex];
            if (node == null || string.IsNullOrWhiteSpace(node.NodeId) ||
                !state.IsNodePurchased(node.NodeId) ||
                node.NodeType == Chaosbound.SkillTree.SkillTreeNodeType.Root)
                continue;

            string optionId = string.Empty;
            string optionName = string.Empty;
            SkillTreeEffect[] effects = node.Effects;

            if (node.NodeType == Chaosbound.SkillTree.SkillTreeNodeType.Choice)
            {
                if (!state.TryGetSelectedChoice(node.NodeId, out optionId))
                    continue;

                ChoiceOption selected = null;
                if (node.ChoiceOptions != null)
                {
                    for (int i = 0; i < node.ChoiceOptions.Length; i++)
                    {
                        ChoiceOption candidate = node.ChoiceOptions[i];
                        if (candidate != null && candidate.OptionId == optionId)
                        {
                            selected = candidate;
                            break;
                        }
                    }
                }

                if (selected == null)
                    continue;

                optionName = selected.DisplayName ?? string.Empty;
                effects = selected.Effects;
            }

            if (effects == null)
                continue;

            for (int effectIndex = 0; effectIndex < effects.Length; effectIndex++)
            {
                SkillTreeEffect effect = effects[effectIndex];
                if (effect == null)
                    continue;

                PlayerStatEffect statEffect = effect as PlayerStatEffect;
                if (playerStats && statEffect != null)
                {
                    ExpeditionTelemetryCollector.RegisterSkillTreeEffect(
                        node.NodeId, node.DisplayName, optionId, optionName,
                        effectIndex, effect.GetType().Name, "PlayerStats",
                        statEffect.StatType.ToString(),
                        statEffect.ModifierType.ToString(), statEffect.Value);
                    continue;
                }

                GlobalSkillModifierEffect skillEffect =
                    effect as GlobalSkillModifierEffect;
                if (!playerStats && skillEffect != null)
                {
                    ExpeditionTelemetryCollector.RegisterSkillTreeEffect(
                        node.NodeId, node.DisplayName, optionId, optionName,
                        effectIndex, effect.GetType().Name, "GlobalSkillModifiers",
                        "AllSkills", skillEffect.ModifierType.ToString(),
                        skillEffect.Value);
                }
            }
        }
    }
}
#endif
