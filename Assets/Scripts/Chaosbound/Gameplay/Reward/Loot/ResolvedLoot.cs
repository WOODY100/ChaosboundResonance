using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents the complete result of resolving one PendingLoot item.
/// </summary>
public sealed class ResolvedLoot
{
    private readonly IReadOnlyList<ResolvedLootEntry>
        entries;

    /// <summary>
    /// Gets the spatial origin associated with this loot result.
    /// </summary>
    public Vector3 Origin { get; }

    /// <summary>
    /// Gets every successfully resolved loot entry.
    /// </summary>
    public IReadOnlyList<ResolvedLootEntry> Entries =>
        entries;

    /// <summary>
    /// Gets whether this result contains no resolved entries.
    /// </summary>
    public bool IsEmpty =>
        entries.Count == 0;

    /// <summary>
    /// Creates a complete resolved loot result.
    /// </summary>
    public ResolvedLoot(
        Vector3 origin,
        IEnumerable<ResolvedLootEntry> entries)
    {
        if (entries == null)
        {
            throw new ArgumentNullException(
                nameof(entries));
        }

        List<ResolvedLootEntry> list =
            entries.ToList();

        if (list.Any(entry => entry == null))
        {
            throw new ArgumentException(
                "ResolvedLoot cannot contain null entries.",
                nameof(entries));
        }

        Origin =
            origin;

        this.entries =
            new ReadOnlyCollection<ResolvedLootEntry>(
                list);
    }
}