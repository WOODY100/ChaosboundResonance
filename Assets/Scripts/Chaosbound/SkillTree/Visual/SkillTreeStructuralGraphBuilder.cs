using Chaosbound.SkillTree;
using System.Collections.Generic;

public static class SkillTreeStructuralGraphBuilder
{
    public static IReadOnlyList<SkillTreeConnectionDescriptor> Build(
        SkillTreeDatabase database)
    {
        List<SkillTreeConnectionDescriptor> connections =
            new List<SkillTreeConnectionDescriptor>();

        if (database == null)
            return connections;

        IReadOnlyList<SkillTreeNodeDefinition> nodes =
            database.Nodes;

        if (nodes == null)
            return connections;

        SkillTreeNodeDefinition rootNode = null;

        for (int i = 0; i < nodes.Count; i++)
        {
            SkillTreeNodeDefinition node = nodes[i];

            if (node == null)
                continue;

            if (node.NodeType == SkillTreeNodeType.Root)
            {
                rootNode = node;
                break;
            }
        }

        if (rootNode == null)
            return connections;

        for (int i = 0; i < nodes.Count; i++)
        {
            SkillTreeNodeDefinition node = nodes[i];

            if (node == null)
                continue;

            if (node.NodeType != SkillTreeNodeType.Gate)
                continue;

            if (node.Branch == SkillTreeBranch.None)
                continue;

            if (node.Requirement != null)
                continue;

            connections.Add(
                new SkillTreeConnectionDescriptor(
                    rootNode.NodeId,
                    node.NodeId,
                    SkillTreeConnectionKind.StructuralBranchEntry));
        }

        return connections;
    }
}