using System;

[Serializable]
public readonly struct SkillTreeConnectionDescriptor
{
    public readonly string SourceNodeId;
    public readonly string TargetNodeId;
    public readonly SkillTreeConnectionKind Kind;

    public SkillTreeConnectionDescriptor(
        string sourceNodeId,
        string targetNodeId,
        SkillTreeConnectionKind kind)
    {
        SourceNodeId = sourceNodeId;
        TargetNodeId = targetNodeId;
        Kind = kind;
    }
}