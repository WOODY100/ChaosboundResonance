using System;
using UnityEngine;

/// <summary>
/// Represents pending loot work submitted by a Loot Bag
/// to the Reward Runtime.
/// </summary>
public sealed class PendingLoot
{
    /// <summary>
    /// Gets the loot definition that must be resolved.
    /// </summary>
    public LootDefinition LootDefinition { get; }

    /// <summary>
    /// Gets the spatial origin captured when the Loot Bag opened.
    /// </summary>
    public Vector3 Origin { get; }

    /// <summary>
    /// Creates a new pending loot work item.
    /// </summary>
    public PendingLoot(
        LootDefinition lootDefinition,
        Vector3 origin)
    {
        LootDefinition =
            lootDefinition
            ?? throw new ArgumentNullException(
                nameof(lootDefinition));

        Origin =
            origin;
    }
}