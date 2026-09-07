using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    menuName = "Chaosbound/Loot/Loot Definition")]
public sealed class LootDefinition :
    ScriptableObject
{
    [SerializeField]
    private List<LootEntryDefinition> m_Entries =
        new();

    public IReadOnlyList<LootEntryDefinition> Entries =>
        m_Entries;

    private void OnValidate()
    {
        m_Entries ??=
            new List<LootEntryDefinition>();
    }
}