using Chaosbound.Content.Expeditions.Enums.Spawn;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.Spawn.Content;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Reference.Models;
using Chaosbound.Gameplay.Spawn.References;
using System;
using System.Collections.Generic;

public sealed class RewardSpawnRequestBuilder
{
    private readonly MaterializableContentResolver
        contentResolver;

    private readonly SpawnRequestEntryFactory
        entryFactory;

    private readonly SpawnRequestFactory
        requestFactory;

    public RewardSpawnRequestBuilder(
        MaterializableContentResolver contentResolver)
        : this(
            contentResolver,
            new SpawnRequestEntryFactory(
                new MaterializableReferenceFactory()),
            new SpawnRequestFactory())
    {
    }

    public RewardSpawnRequestBuilder(
        MaterializableContentResolver contentResolver,
        SpawnRequestEntryFactory entryFactory,
        SpawnRequestFactory requestFactory)
    {
        this.contentResolver =
            contentResolver
            ?? throw new ArgumentNullException(
                nameof(contentResolver));

        this.entryFactory =
            entryFactory
            ?? throw new ArgumentNullException(
                nameof(entryFactory));

        this.requestFactory =
            requestFactory
            ?? throw new ArgumentNullException(
                nameof(requestFactory));
    }

    public SpawnRequest Build(
        ResolvedLoot resolvedLoot,
        RuntimeSpawnConfig spawnConfig)
    {
        if (resolvedLoot == null)
        {
            throw new ArgumentNullException(
                nameof(resolvedLoot));
        }

        if (spawnConfig == null)
        {
            throw new ArgumentNullException(
                nameof(spawnConfig));
        }

        List<SpawnRequestEntry> entries =
            new List<SpawnRequestEntry>();

        foreach (ResolvedLootEntry lootEntry
            in resolvedLoot.Entries)
        {
            if (lootEntry == null)
                continue;

            switch (lootEntry.Type)
            {
                case LootEntryType.Resource:
                    entries.Add(
                        BuildResourceEntry(
                            lootEntry));
                    break;

                case LootEntryType.Item:
                    entries.Add(
                        BuildItemEntry(
                            lootEntry));
                    break;

                case LootEntryType.Currency:
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unsupported loot entry type " +
                        $"'{lootEntry.Type}'.");
            }
        }

        RuntimeSpawnConfig rewardSpawnConfig =
            spawnConfig.WithPlacement(
                SpawnPlacementPolicy.AroundOrigin);

        SpawnSpatialOrigin spatialOrigin =
            new SpawnSpatialOrigin(
                resolvedLoot.Origin);

        return requestFactory.Create(
            entries,
            rewardSpawnConfig,
            SpawnRequestOrigin.Loot,
            spatialOrigin);
    }

    private SpawnRequestEntry
        BuildResourceEntry(
            ResolvedLootEntry lootEntry)
    {
        if (lootEntry.Amount <= 0)
        {
            throw new InvalidOperationException(
                $"Resource loot '{lootEntry.ContentId}' " +
                "has an invalid amount " +
                $"'{lootEntry.Amount}'.");
        }

        MaterializableContentDefinition definition =
            contentResolver.Resolve(
                lootEntry.ContentId);

        ResourceMaterializableReference reference =
            new ResourceMaterializableReference(
                lootEntry.ContentId,
                definition.Prefab,
                lootEntry.Amount);

        return entryFactory.Create(
            reference,
            1);
    }

    private SpawnRequestEntry
        BuildItemEntry(
            ResolvedLootEntry lootEntry)
    {
        if (lootEntry.Amount <= 0)
        {
            throw new InvalidOperationException(
                $"Item loot '{lootEntry.ContentId}' " +
                $"has an invalid amount " +
                $"'{lootEntry.Amount}'.");
        }

        MaterializableContentDefinition definition =
            contentResolver.Resolve(
                lootEntry.ContentId);

        ItemMaterializableReference reference =
            new ItemMaterializableReference(
                lootEntry.ContentId,
                definition.Prefab);

        return entryFactory.Create(
            reference,
            lootEntry.Amount);
    }
}