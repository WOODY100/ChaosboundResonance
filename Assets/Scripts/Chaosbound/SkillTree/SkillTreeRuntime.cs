using Chaosbound.Gameplay.MetaProgression.Persistent;
using Chaosbound.SkillTree;
using System;

public sealed class SkillTreeRuntime
{
    private readonly SkillTreeDatabase database;
    private readonly SkillTreeRuntimeState state;
    private readonly PersistentMetaRuntime persistentMetaRuntime;

    public SkillTreeDatabase Database
    {
        get { return database; }
    }

    public PersistentMetaRuntime PersistentMetaRuntime
    {
        get { return persistentMetaRuntime; }
    }

    public SkillTreeRuntimeState State
    {
        get { return state; }
    }

    public SkillTreeRuntime(
        SkillTreeDatabase database,
        PersistentMetaRuntime persistentMetaRuntime)
    {
        if (database == null)
            throw new ArgumentNullException(nameof(database));

        if (persistentMetaRuntime == null)
            throw new ArgumentNullException(
                nameof(persistentMetaRuntime));

        this.database = database;
        this.persistentMetaRuntime = persistentMetaRuntime;
        this.state = new SkillTreeRuntimeState();
    }

    public SkillTreeNodeDefinition GetNode(string nodeId)
    {
        return database.GetNode(nodeId);
    }

    public SkillTreeVisualState GetNodeState(string nodeId)
    {
        SkillTreeNodeDefinition node = GetNode(nodeId);

        if (node == null)
            return SkillTreeVisualState.Locked;

        if (IsRoot(node))
            return SkillTreeVisualState.Purchased;

        if (state.IsNodePurchased(node.NodeId))
            return SkillTreeVisualState.Purchased;

        if (!EvaluateRequirement(node.Requirement))
            return SkillTreeVisualState.Locked;

        if (node.Cost < 0)
            return SkillTreeVisualState.Locked;

        if (persistentMetaRuntime.State.SkillPoints < node.Cost)
            return SkillTreeVisualState.Locked;

        return SkillTreeVisualState.Available;
    }

    public bool CanPurchase(string nodeId)
    {
        SkillTreeNodeDefinition node = GetNode(nodeId);

        if (node == null)
            return false;

        if (IsRoot(node))
            return false;

        if (state.IsNodePurchased(node.NodeId))
            return false;

        if (!EvaluateRequirement(node.Requirement))
            return false;

        if (node.NodeType == SkillTreeNodeType.Choice)
            return false;

        if (node.Cost < 0)
            return false;

        if (persistentMetaRuntime.State.SkillPoints < node.Cost)
            return false;

        return true;
    }

    public bool PurchaseNode(string nodeId)
    {
        if (!CanPurchase(nodeId))
            return false;

        SkillTreeNodeDefinition node = GetNode(nodeId);

        if (!persistentMetaRuntime.TrySpendSkillPoints(node.Cost))
            return false;

        return state.AddPurchasedNode(node.NodeId);
    }

    public bool CanSelectChoice(
        string nodeId,
        string optionId)
    {
        SkillTreeNodeDefinition node = GetNode(nodeId);

        if (node == null)
            return false;

        if (node.NodeType != SkillTreeNodeType.Choice)
            return false;

        if (state.IsNodePurchased(node.NodeId))
            return false;

        if (!EvaluateRequirement(node.Requirement))
            return false;

        if (node.Cost < 0)
            return false;

        if (persistentMetaRuntime.State.SkillPoints < node.Cost)
            return false;

        if (string.IsNullOrWhiteSpace(optionId))
            return false;

        if (node.ChoiceOptions == null)
            return false;

        for (int i = 0; i < node.ChoiceOptions.Length; i++)
        {
            if (node.ChoiceOptions[i] == null)
                continue;

            if (node.ChoiceOptions[i].OptionId == optionId)
                return true;
        }

        return false;
    }

    public bool PurchaseChoice(
    string nodeId,
    string optionId)
    {
        if (!CanSelectChoice(nodeId, optionId))
            return false;

        SkillTreeNodeDefinition node =
            GetNode(nodeId);

        if (!persistentMetaRuntime.TrySpendSkillPoints(node.Cost))
            return false;

        /*
         * Choice purchase is atomic:
         *
         * 1. Spend the node cost.
         * 2. Register selected option.
         * 3. Register node as purchased.
         *
         * CanSelectChoice() already validated
         * the node, option and available Skill Points.
         */

        if (!state.SetSelectedChoice(
            node.NodeId,
            optionId))
            return false;

        if (!state.AddPurchasedNode(
            node.NodeId))
            return false;

        return true;
    }

    private bool EvaluateRequirement(NodeRequirement requirement)
    {
        if (requirement == null)
            return true;

        NodePurchasedRequirement purchasedRequirement =
            requirement as NodePurchasedRequirement;

        if (purchasedRequirement != null)
        {
            if (string.IsNullOrWhiteSpace(
                purchasedRequirement.NodeId))
            {
                return false;
            }

            return IsRequirementNodeSatisfied(
                purchasedRequirement.NodeId);
        }

        AllRequirement allRequirement =
            requirement as AllRequirement;

        if (allRequirement != null)
        {
            if (allRequirement.Requirements == null ||
                allRequirement.Requirements.Length == 0)
            {
                return false;
            }

            for (int i = 0;
                 i < allRequirement.Requirements.Length;
                 i++)
            {
                if (!EvaluateRequirement(
                    allRequirement.Requirements[i]))
                {
                    return false;
                }
            }

            return true;
        }

        AnyRequirement anyRequirement =
            requirement as AnyRequirement;

        if (anyRequirement != null)
        {
            if (anyRequirement.Requirements == null ||
                anyRequirement.Requirements.Length == 0)
            {
                return false;
            }

            for (int i = 0;
                 i < anyRequirement.Requirements.Length;
                 i++)
            {
                if (EvaluateRequirement(
                    anyRequirement.Requirements[i]))
                {
                    return true;
                }
            }

            return false;
        }

        return false;
    }

    private bool IsRequirementNodeSatisfied(string nodeId)
    {
        SkillTreeNodeDefinition node =
            database.GetNode(nodeId);

        if (node == null)
            return false;

        if (IsRoot(node))
            return true;

        return state.IsNodePurchased(nodeId);
    }

    private bool IsRoot(SkillTreeNodeDefinition node)
    {
        return node != null &&
               node.NodeType == SkillTreeNodeType.Root;
    }
}