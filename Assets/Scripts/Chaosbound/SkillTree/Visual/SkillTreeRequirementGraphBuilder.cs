using System.Collections.Generic;

public static class SkillTreeRequirementGraphBuilder
{
    public static List<SkillTreeConnectionDescriptor> Build(
        SkillTreeDatabase database)
    {
        var connections =
            new List<SkillTreeConnectionDescriptor>();

        if (database == null)
            return connections;

        IReadOnlyList<SkillTreeNodeDefinition> nodes =
            database.Nodes;

        if (nodes == null)
            return connections;

        var duplicateGuard =
            new HashSet<string>();

        for (int i = 0; i < nodes.Count; i++)
        {
            SkillTreeNodeDefinition targetNode =
                nodes[i];

            if (targetNode == null)
                continue;

            if (string.IsNullOrWhiteSpace(targetNode.NodeId))
                continue;

            NodeRequirement requirement =
                targetNode.Requirement;

            if (requirement == null)
                continue;

            CollectConnections(
                requirement,
                targetNode.NodeId,
                connections,
                duplicateGuard);
        }

        return connections;
    }

    private static void CollectConnections(
        NodeRequirement requirement,
        string targetNodeId,
        List<SkillTreeConnectionDescriptor> connections,
        HashSet<string> duplicateGuard)
    {
        if (requirement == null)
            return;

        NodePurchasedRequirement purchasedRequirement =
            requirement as NodePurchasedRequirement;

        if (purchasedRequirement != null)
        {
            AddConnection(
                purchasedRequirement.NodeId,
                targetNodeId,
                connections,
                duplicateGuard);

            return;
        }

        AllRequirement allRequirement =
            requirement as AllRequirement;

        if (allRequirement != null)
        {
            CollectChildren(
                allRequirement.Requirements,
                targetNodeId,
                connections,
                duplicateGuard);

            return;
        }

        AnyRequirement anyRequirement =
            requirement as AnyRequirement;

        if (anyRequirement != null)
        {
            CollectChildren(
                anyRequirement.Requirements,
                targetNodeId,
                connections,
                duplicateGuard);
        }
    }

    private static void CollectChildren(
        NodeRequirement[] requirements,
        string targetNodeId,
        List<SkillTreeConnectionDescriptor> connections,
        HashSet<string> duplicateGuard)
    {
        if (requirements == null)
            return;

        for (int i = 0; i < requirements.Length; i++)
        {
            NodeRequirement child =
                requirements[i];

            CollectConnections(
                child,
                targetNodeId,
                connections,
                duplicateGuard);
        }
    }

    private static void AddConnection(
        string sourceNodeId,
        string targetNodeId,
        List<SkillTreeConnectionDescriptor> connections,
        HashSet<string> duplicateGuard)
    {
        if (string.IsNullOrWhiteSpace(sourceNodeId))
            return;

        if (string.IsNullOrWhiteSpace(targetNodeId))
            return;

        string key =
            sourceNodeId + "->" + targetNodeId;

        if (!duplicateGuard.Add(key))
            return;

        connections.Add(
            new SkillTreeConnectionDescriptor(
                sourceNodeId,
                targetNodeId,
                SkillTreeConnectionKind.Prerequisite));
    }
}