using System.Collections.Generic;

public class ModifierSource
{
    public string SourceID;
    public List<StatModifier> Modifiers = new();

    public ModifierSource(string id)
    {
        SourceID = id;
    }
}