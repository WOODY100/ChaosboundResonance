using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    menuName = "Chaosbound/Content/Materializable Content Database")]
public sealed class MaterializableContentDatabase : ScriptableObject
{
    [SerializeField]
    private List<MaterializableContentDefinition> entries =
        new List<MaterializableContentDefinition>();

    public IReadOnlyList<MaterializableContentDefinition> Entries =>
        entries;

    public int Count => entries.Count;

    public bool TryGet(
        string contentId,
        out MaterializableContentDefinition definition)
    {
        definition = null;

        if (string.IsNullOrEmpty(contentId))
            return false;

        for (int i = 0; i < entries.Count; i++)
        {
            MaterializableContentDefinition entry = entries[i];

            if (entry == null)
                continue;

            if (entry.ContentId == contentId)
            {
                definition = entry;
                return true;
            }
        }

        return false;
    }

    private void OnValidate()
    {
        entries.RemoveAll(entry => entry == null);

        HashSet<string> contentIds =
            new HashSet<string>();

        for (int i = 0; i < entries.Count; i++)
        {
            MaterializableContentDefinition entry =
                entries[i];

            if (string.IsNullOrEmpty(entry.ContentId))
            {
                Debug.LogError(
                    $"[{nameof(MaterializableContentDatabase)}] " +
                    $"Entry at index {i} has an empty ContentId.",
                    this);

                continue;
            }

            if (entry.Prefab == null)
            {
                Debug.LogError(
                    $"[{nameof(MaterializableContentDatabase)}] " +
                    $"ContentId '{entry.ContentId}' has no prefab assigned.",
                    entry);

                continue;
            }

            if (!contentIds.Add(entry.ContentId))
            {
                Debug.LogError(
                    $"[{nameof(MaterializableContentDatabase)}] " +
                    $"Duplicate ContentId detected: '{entry.ContentId}'.",
                    this);
            }
        }
    }
}