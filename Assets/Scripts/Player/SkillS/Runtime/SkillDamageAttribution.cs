#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>Editor-only identity carried by player skill damage for Analytics.</summary>
public static class SkillDamageAttribution
{
    private static readonly Dictionary<SkillDefinition, string> skillIds =
        new Dictionary<SkillDefinition, string>();

    public static string CreateExecutionId()
    {
        return Guid.NewGuid().ToString("N");
    }

    public static string GetReadableSkillId(SkillDefinition definition)
    {
        if (definition == null)
            return string.Empty;

        return string.IsNullOrWhiteSpace(definition.AnalyticsId)
            ? definition.name.ToLowerInvariant().Replace(" ", "_")
            : definition.AnalyticsId;
    }

    public static string GetSkillDisplayName(SkillDefinition definition)
    {
        if (definition == null)
            return string.Empty;

        return string.IsNullOrWhiteSpace(definition.DisplayName)
            ? definition.name
            : definition.DisplayName;
    }

    public static string GetSkillDefinitionId(SkillDefinition definition)
    {
        if (definition == null)
            return string.Empty;

        string cachedId;
        if (skillIds.TryGetValue(definition, out cachedId))
            return cachedId;

        string path = AssetDatabase.GetAssetPath(definition);
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        string id = AssetDatabase.AssetPathToGUID(path);
        skillIds.Add(definition, id);
        return id;
    }
}
#endif
