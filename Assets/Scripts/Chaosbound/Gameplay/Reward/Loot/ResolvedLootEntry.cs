using System;

/// <summary>
/// Represents one successfully resolved loot entry.
/// </summary>
public sealed class ResolvedLootEntry
{
    /// <summary>
    /// Gets the resolved loot type.
    /// </summary>
    public LootEntryType Type { get; }

    /// <summary>
    /// Gets the resolved content identifier.
    /// </summary>
    public string ContentId { get; }

    /// <summary>
    /// Gets the resolved amount.
    /// The meaning of this amount is interpreted downstream.
    /// </summary>
    public int Amount { get; }

    /// <summary>
    /// Creates a resolved loot entry.
    /// </summary>
    public ResolvedLootEntry(
        LootEntryType type,
        string contentId,
        int amount)
    {
        if (string.IsNullOrWhiteSpace(contentId))
        {
            throw new ArgumentException(
                "Content ID cannot be empty.",
                nameof(contentId));
        }

        Type =
            type;

        ContentId =
            contentId.Trim();

        Amount =
            amount;
    }
}