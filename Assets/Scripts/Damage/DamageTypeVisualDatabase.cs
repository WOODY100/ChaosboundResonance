using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DamageTypeVisualDatabase : ScriptableObject
{
    public List<DamageTypeColorEntry> entries;

    public Color GetColor(DamageType type)
    {
        foreach (var entry in entries)
        {
            if (entry.type == type)
                return entry.color;
        }

        return Color.white;
    }
}

[System.Serializable]
public class DamageTypeColorEntry
{
    public DamageType type;
    public Color color;
}