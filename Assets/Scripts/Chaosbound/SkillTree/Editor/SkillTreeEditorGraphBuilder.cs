#if UNITY_EDITOR

using System.Collections.Generic;

public sealed class SkillTreeEditorGraphBuilder
{
    public List<SkillTreeConnectionDescriptor> Build(
        SkillTreeDatabase database)
    {
        List<SkillTreeConnectionDescriptor> result =
            new List<SkillTreeConnectionDescriptor>();

        if (database == null)
            return result;

        IReadOnlyList<SkillTreeConnectionDescriptor>
            prerequisiteConnections =
                SkillTreeRequirementGraphBuilder.Build(
                    database);

        if (prerequisiteConnections != null)
        {
            for (int i = 0;
                 i < prerequisiteConnections.Count;
                 i++)
            {
                result.Add(
                    prerequisiteConnections[i]);
            }
        }

        IReadOnlyList<SkillTreeConnectionDescriptor>
            structuralConnections =
                SkillTreeStructuralGraphBuilder.Build(
                    database);

        if (structuralConnections != null)
        {
            for (int i = 0;
                 i < structuralConnections.Count;
                 i++)
            {
                result.Add(
                    structuralConnections[i]);
            }
        }

        return result;
    }
}

#endif