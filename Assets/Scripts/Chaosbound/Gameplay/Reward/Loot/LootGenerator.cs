using System;
using System.Collections.Generic;

public sealed class LootGenerator
{
    private readonly ILootRandom random;

    public LootGenerator(
        ILootRandom random)
    {
        this.random =
            random
            ?? throw new ArgumentNullException(
                nameof(random));
    }

    public ResolvedLoot Generate(
        LootDefinition definition,
        UnityEngine.Vector3 origin)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(
                nameof(definition));
        }

        List<ResolvedLootEntry> entries =
            new List<ResolvedLootEntry>();

        foreach (LootEntryDefinition entry
            in definition.Entries)
        {
            if (entry == null)
                continue;

            if (!RollChance(entry.Chance))
                continue;

            entries.Add(
                new ResolvedLootEntry(
                    entry.Type,
                    entry.ContentId,
                    entry.Amount));
        }

        return new ResolvedLoot(
            origin,
            entries);
    }

    private bool RollChance(float chance)
    {
        chance =
            UnityEngine.Mathf.Clamp01(chance);

        if (chance <= 0f)
            return false;

        if (chance >= 1f)
            return true;

        return random.Value() <= chance;
    }
}