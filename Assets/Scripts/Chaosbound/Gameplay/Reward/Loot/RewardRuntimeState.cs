using System;
using System.Collections.Generic;

/// <summary>
/// Persistent runtime state owned by the Reward Domain.
/// </summary>
public sealed class RewardRuntimeState
{
    private readonly Queue<PendingLoot>
        pendingLoot =
            new Queue<PendingLoot>();

    private readonly Queue<ResolvedLoot>
        resolvedLoot =
            new Queue<ResolvedLoot>();

    /// <summary>
    /// Gets whether pending loot work is available.
    /// </summary>
    public bool HasPendingLoot =>
        pendingLoot.Count > 0;

    /// <summary>
    /// Gets whether resolved loot is waiting
    /// for downstream consumption.
    /// </summary>
    public bool HasResolvedLoot =>
        resolvedLoot.Count > 0;

    /// <summary>
    /// Adds pending loot work to the FIFO queue.
    /// </summary>
    public void AddPendingLoot(
        PendingLoot loot)
    {
        if (loot == null)
        {
            throw new ArgumentNullException(
                nameof(loot));
        }

        pendingLoot.Enqueue(
            loot);
    }

    /// <summary>
    /// Attempts to dequeue the next pending loot item.
    /// </summary>
    public bool TryDequeuePendingLoot(
        out PendingLoot loot)
    {
        return pendingLoot.TryDequeue(
            out loot);
    }

    /// <summary>
    /// Adds a complete resolved loot result
    /// to the FIFO queue.
    /// </summary>
    public void AddResolvedLoot(
        ResolvedLoot loot)
    {
        if (loot == null)
        {
            throw new ArgumentNullException(
                nameof(loot));
        }

        resolvedLoot.Enqueue(
            loot);
    }

    /// <summary>
    /// Attempts to inspect the next resolved loot
    /// without removing it.
    /// </summary>
    public bool TryPeekResolvedLoot(
        out ResolvedLoot loot)
    {
        return resolvedLoot.TryPeek(
            out loot);
    }

    /// <summary>
    /// Consumes the current resolved loot queue head.
    /// </summary>
    public void ConsumeResolvedLoot()
    {
        if (resolvedLoot.Count == 0)
        {
            throw new InvalidOperationException(
                "Cannot consume resolved loot because " +
                "the queue is empty.");
        }

        resolvedLoot.Dequeue();
    }
}