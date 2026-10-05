using Chaosbound.SkillTree;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SkillTreeDatabase",
    menuName = "Chaosbound/Skill Tree/Database"
)]
public sealed class SkillTreeDatabase : ScriptableObject
{
    [Header("Identity")]
    [SerializeField]
    private string treeId;

    [Header("Nodes")]
    [SerializeField]
    private SkillTreeNodeDefinition[] nodes;

    private Dictionary<string, SkillTreeNodeDefinition> nodeLookup;

    public string TreeId => treeId;

    public IReadOnlyList<SkillTreeNodeDefinition> Nodes => nodes;

    public SkillTreeNodeDefinition GetNode(string nodeId)
    {
        if (string.IsNullOrEmpty(nodeId))
            return null;

        EnsureLookup();

        nodeLookup.TryGetValue(nodeId, out SkillTreeNodeDefinition node);
        return node;
    }

    public bool TryGetNode(
        string nodeId,
        out SkillTreeNodeDefinition node)
    {
        node = GetNode(nodeId);
        return node != null;
    }

    public bool Validate()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(treeId))
        {
            Debug.LogError(
                $"[{nameof(SkillTreeDatabase)}] TreeId is empty.",
                this);

            isValid = false;
        }

        if (nodes == null || nodes.Length == 0)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeDatabase)}] Nodes array is empty.",
                this);

            return false;
        }

        HashSet<string> nodeIds = new HashSet<string>();

        for (int i = 0; i < nodes.Length; i++)
        {
            SkillTreeNodeDefinition node = nodes[i];

            if (node == null)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Node at index {i} is null.",
                    this);

                isValid = false;
                continue;
            }

            if (string.IsNullOrWhiteSpace(node.NodeId))
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Node at index {i} has an empty NodeId.",
                    node);

                isValid = false;
                continue;
            }

            if (!nodeIds.Add(node.NodeId))
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Duplicate NodeId detected: '{node.NodeId}'.",
                    node);

                isValid = false;
            }

            if (!ValidateNodeCoherence(node))
            {
                isValid = false;
            }

            if (!ValidateNodeBranch(node))
            {
                isValid = false;
            }

            if (!ValidateNodeRequirements(node))
            {
                isValid = false;
            }

            if (!ValidateChoiceOptions(node))
            {
                isValid = false;
            }

            if (!ValidateEffects(
                node.Effects,
                node.NodeId,
                node))
            {
                isValid = false;
            }

            if (!ValidateChoiceOptionEffects(node))
            {
                isValid = false;
            }
        }

        if (!ValidateRootNode())
        {
            isValid = false;
        }

        if (!ValidateRequirementCycles())
        {
            isValid = false;
        }

        return isValid;
    }

    private bool ValidateNodeCoherence(
        SkillTreeNodeDefinition node)
    {
        bool isValid = true;

        if (node.Cost < 0)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeDatabase)}] Node '{node.NodeId}' has a negative Cost.",
                node);

            isValid = false;
        }

        if (node.NodeType == SkillTreeNodeType.Root)
        {
            if (node.Branch != SkillTreeBranch.None)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] ROOT node '{node.NodeId}' must use Branch.None.",
                    node);

                isValid = false;
            }

            if (node.Cost != 0)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] ROOT node '{node.NodeId}' must have Cost = 0.",
                    node);

                isValid = false;
            }

            if (node.Requirement != null)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] ROOT node '{node.NodeId}' cannot have a Requirement.",
                    node);

                isValid = false;
            }

            if (node.Effects != null && node.Effects.Length > 0)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] ROOT node '{node.NodeId}' cannot have Effects.",
                    node);

                isValid = false;
            }

            if (node.ChoiceOptions != null &&
                node.ChoiceOptions.Length > 0)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] ROOT node '{node.NodeId}' cannot have ChoiceOptions.",
                    node);

                isValid = false;
            }
        }

        if (node.NodeType == SkillTreeNodeType.Choice)
        {
            if (node.ChoiceOptions == null ||
                node.ChoiceOptions.Length < 2)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Choice node '{node.NodeId}' must have at least 2 ChoiceOptions.",
                    node);

                isValid = false;
            }

            if (node.Effects != null &&
                node.Effects.Length > 0)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Choice node '{node.NodeId}' cannot have direct Effects. Effects belong to ChoiceOptions.",
                    node);

                isValid = false;
            }
        }
        else
        {
            if (node.ChoiceOptions != null &&
                node.ChoiceOptions.Length > 0)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Non-Choice node '{node.NodeId}' cannot have ChoiceOptions.",
                    node);

                isValid = false;
            }
        }

        return isValid;
    }

    private void EnsureLookup()
    {
        if (nodeLookup != null)
            return;

        nodeLookup = new Dictionary<string, SkillTreeNodeDefinition>();

        if (nodes == null)
            return;

        foreach (SkillTreeNodeDefinition node in nodes)
        {
            if (node == null)
                continue;

            if (string.IsNullOrEmpty(node.NodeId))
                continue;

            if (nodeLookup.ContainsKey(node.NodeId))
                continue;

            nodeLookup.Add(node.NodeId, node);
        }
    }

    private bool ValidateRootNode()
    {
        int rootCount = 0;
        SkillTreeNodeDefinition rootNode = null;

        if (nodes == null)
            return false;

        foreach (SkillTreeNodeDefinition node in nodes)
        {
            if (node == null)
                continue;

            if (node.NodeType != SkillTreeNodeType.Root)
                continue;

            rootCount++;
            rootNode = node;
        }

        if (rootCount == 0)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeDatabase)}] Skill Tree must contain exactly one ROOT node, but none was found.",
                this);

            return false;
        }

        if (rootCount > 1)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeDatabase)}] Skill Tree must contain exactly one ROOT node, but {rootCount} were found.",
                this);

            return false;
        }

        if (rootNode == null)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeDatabase)}] ROOT node reference could not be resolved.",
                this);

            return false;
        }

        return true;
    }

    private bool ValidateRequirement(
        NodeRequirement requirement,
        string ownerNodeId)
    {
        if (requirement == null)
            return true;

        if (requirement is NodePurchasedRequirement purchasedRequirement)
        {
            if (string.IsNullOrWhiteSpace(purchasedRequirement.NodeId))
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Node '{ownerNodeId}' has a NodePurchasedRequirement with an empty NodeId.",
                    this);

                return false;
            }

            if (purchasedRequirement.NodeId == ownerNodeId)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Node '{ownerNodeId}' cannot require itself.",
                    this);

                return false;
            }

            if (!TryGetNode(purchasedRequirement.NodeId, out _))
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Node '{ownerNodeId}' references missing required NodeId '{purchasedRequirement.NodeId}'.",
                    this);

                return false;
            }

            return true;
        }

        if (requirement is AllRequirement allRequirement)
        {
            if (allRequirement.Requirements == null ||
                allRequirement.Requirements.Length == 0)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Node '{ownerNodeId}' contains an empty AllRequirement.",
                    this);

                return false;
            }

            bool isValid = true;

            foreach (NodeRequirement child in allRequirement.Requirements)
            {
                if (!ValidateRequirement(child, ownerNodeId))
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        if (requirement is AnyRequirement anyRequirement)
        {
            if (anyRequirement.Requirements == null ||
                anyRequirement.Requirements.Length == 0)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Node '{ownerNodeId}' contains an empty AnyRequirement.",
                    this);

                return false;
            }

            bool isValid = true;

            foreach (NodeRequirement child in anyRequirement.Requirements)
            {
                if (!ValidateRequirement(child, ownerNodeId))
                {
                    isValid = false;
                }
            }

            return isValid;
        }

        Debug.LogError(
            $"[{nameof(SkillTreeDatabase)}] Node '{ownerNodeId}' contains an unsupported requirement type: {requirement.GetType().Name}.",
            this);

        return false;
    }

    private bool ValidateNodeRequirements(
    SkillTreeNodeDefinition node)
    {
        if (node.Requirement == null)
            return true;

        return ValidateRequirement(
            node.Requirement,
            node.NodeId);
    }

    private bool ValidateRequirementCycles()
    {
        bool isValid = true;

        if (nodes == null)
            return false;

        foreach (SkillTreeNodeDefinition node in nodes)
        {
            if (node == null || node.Requirement == null)
                continue;

            HashSet<string> visiting = new HashSet<string>();
            HashSet<string> visited = new HashSet<string>();

            if (HasRequirementCycle(
                    node.Requirement,
                    visiting,
                    visited,
                    node.NodeId))
            {
                isValid = false;
            }
        }

        return isValid;
    }

    private bool HasRequirementCycle(
        NodeRequirement requirement,
        HashSet<string> visiting,
        HashSet<string> visited,
        string ownerNodeId)
    {
        if (requirement == null)
            return false;

        if (requirement is NodePurchasedRequirement purchasedRequirement)
        {
            string requiredNodeId = purchasedRequirement.NodeId;

            if (string.IsNullOrWhiteSpace(requiredNodeId))
                return false;

            if (requiredNodeId == ownerNodeId)
                return true;

            if (visiting.Contains(requiredNodeId))
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Requirement cycle detected involving node '{requiredNodeId}'.",
                    this);

                return true;
            }

            if (visited.Contains(requiredNodeId))
                return false;

            SkillTreeNodeDefinition requiredNode = GetNode(requiredNodeId);

            if (requiredNode == null)
                return false;

            visiting.Add(requiredNodeId);

            bool hasCycle = HasRequirementCycle(
                requiredNode.Requirement,
                visiting,
                visited,
                requiredNodeId);

            visiting.Remove(requiredNodeId);
            visited.Add(requiredNodeId);

            return hasCycle;
        }

        if (requirement is AllRequirement allRequirement)
        {
            if (allRequirement.Requirements == null)
                return false;

            foreach (NodeRequirement child in allRequirement.Requirements)
            {
                if (HasRequirementCycle(
                        child,
                        visiting,
                        visited,
                        ownerNodeId))
                {
                    return true;
                }
            }

            return false;
        }

        if (requirement is AnyRequirement anyRequirement)
        {
            if (anyRequirement.Requirements == null)
                return false;

            foreach (NodeRequirement child in anyRequirement.Requirements)
            {
                if (HasRequirementCycle(
                        child,
                        visiting,
                        visited,
                        ownerNodeId))
                {
                    return true;
                }
            }

            return false;
        }

        return false;
    }

    private bool ValidateChoiceOptions(
        SkillTreeNodeDefinition node)
    {
        if (node.NodeType != SkillTreeNodeType.Choice)
            return true;

        if (node.ChoiceOptions == null ||
            node.ChoiceOptions.Length < 2)
        {
            return false;
        }

        bool isValid = true;

        HashSet<string> optionIds = new HashSet<string>();

        for (int i = 0; i < node.ChoiceOptions.Length; i++)
        {
            ChoiceOption option = node.ChoiceOptions[i];

            if (option == null)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Choice node '{node.NodeId}' has a null ChoiceOption at index {i}.",
                    node);

                isValid = false;
                continue;
            }

            if (string.IsNullOrWhiteSpace(option.OptionId))
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Choice node '{node.NodeId}' has a ChoiceOption with an empty OptionId.",
                    node);

                isValid = false;
                continue;
            }

            if (!optionIds.Add(option.OptionId))
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Choice node '{node.NodeId}' contains duplicate ChoiceOption OptionId '{option.OptionId}'.",
                    node);

                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(option.DisplayName))
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] Choice node '{node.NodeId}' has ChoiceOption '{option.OptionId}' with an empty DisplayName.",
                    node);

                isValid = false;
            }
        }

        return isValid;
    }

    private bool ValidateEffects(
        SkillTreeEffect[] effects,
        string ownerId,
        Object context)
    {
        if (effects == null ||
            effects.Length == 0)
        {
            return true;
        }

        bool isValid = true;

        for (int i = 0; i < effects.Length; i++)
        {
            SkillTreeEffect effect = effects[i];

            if (effect == null)
            {
                Debug.LogError(
                    $"[{nameof(SkillTreeDatabase)}] '{ownerId}' has a null SkillTreeEffect at index {i}.",
                    context);

                isValid = false;
                continue;
            }

            if (effect is PlayerStatEffect)
            {
                continue;
            }

            if (effect is GlobalSkillModifierEffect)
            {
                continue;
            }

            if (effect is RuleEffect ruleEffect)
            {
                if (string.IsNullOrWhiteSpace(ruleEffect.RuleId))
                {
                    Debug.LogError(
                        $"[{nameof(SkillTreeDatabase)}] '{ownerId}' has a RuleEffect with an empty RuleId.",
                        context);

                    isValid = false;
                }

                continue;
            }

            Debug.LogError(
                $"[{nameof(SkillTreeDatabase)}] '{ownerId}' contains an unsupported SkillTreeEffect type: {effect.GetType().Name}.",
                context);

            isValid = false;
        }

        return isValid;
    }

    private bool ValidateChoiceOptionEffects(
    SkillTreeNodeDefinition node)
    {
        if (node.NodeType != SkillTreeNodeType.Choice)
            return true;

        if (node.ChoiceOptions == null)
            return true;

        bool isValid = true;

        foreach (ChoiceOption option in node.ChoiceOptions)
        {
            if (option == null)
                continue;

            if (!ValidateEffects(
                    option.Effects,
                    $"{node.NodeId}/{option.OptionId}",
                    node))
            {
                isValid = false;
            }
        }

        return isValid;
    }

    private bool ValidateNodeBranch(
    SkillTreeNodeDefinition node)
    {
        if (node.NodeType == SkillTreeNodeType.Root)
        {
            return true;
        }

        if (node.Branch == SkillTreeBranch.None)
        {
            Debug.LogError(
                $"[{nameof(SkillTreeDatabase)}] Non-Root node '{node.NodeId}' must have a valid SkillTreeBranch.",
                node);

            return false;
        }

        return true;
    }

    private void OnEnable()
    {
        nodeLookup = null;
    }

    private void OnValidate()
    {
        nodeLookup = null;
    }
}