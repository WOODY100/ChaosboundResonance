using Chaosbound.Content.Expeditions.Enums.Spawn;
using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class RewardDomainDirector
{
    private readonly LootGenerator
        lootGenerator;

    private readonly RewardSpawnRequestBuilder
        spawnRequestBuilder;

    private readonly SpawnRuntime
        spawnRuntime;

    private readonly LootBurstPresentation
        lootBurstPresentation;

    public RewardDomainDirector(
        LootGenerator lootGenerator,
        RewardSpawnRequestBuilder spawnRequestBuilder,
        SpawnRuntime spawnRuntime,
        LootBurstPresentation lootBurstPresentation)
    {
        this.lootGenerator =
            lootGenerator
            ?? throw new ArgumentNullException(
                nameof(lootGenerator));

        this.spawnRequestBuilder =
            spawnRequestBuilder
            ?? throw new ArgumentNullException(
                nameof(spawnRequestBuilder));

        this.spawnRuntime =
            spawnRuntime
            ?? throw new ArgumentNullException(
                nameof(spawnRuntime));

        this.lootBurstPresentation =
            lootBurstPresentation
            ?? throw new ArgumentNullException(
                nameof(lootBurstPresentation));
    }

    public void Execute(
        ExpeditionRuntimeContext context)
    {
        if (context == null)
            throw new ArgumentNullException(
                nameof(context));

        RewardRuntimeState reward =
            context.State.Reward;

        ResolvePendingLoot(
            reward);

        MaterializeResolvedLoot(
            context,
            reward);
    }

    private void ResolvePendingLoot(
        RewardRuntimeState reward)
    {
        while (
            reward.TryDequeuePendingLoot(
                out PendingLoot pendingLoot))
        {
            ResolvedLoot resolvedLoot =
                lootGenerator.Generate(
                    pendingLoot.LootDefinition,
                    pendingLoot.Origin);

            reward.AddResolvedLoot(
                resolvedLoot);
        }
    }

    private void MaterializeResolvedLoot(
        ExpeditionRuntimeContext context,
        RewardRuntimeState reward)
    {
        while (
            reward.TryPeekResolvedLoot(
                out ResolvedLoot resolvedLoot))
        {
            SpawnRequest spawnRequest =
                spawnRequestBuilder.Build(resolvedLoot);

            if (!spawnRequest.IsEmpty)
            {
                RuntimeSpawnConfig rewardSpawnConfig =
                    context.Config.Spawn.WithPlacement(
                        SpawnPlacementPolicy.AroundOrigin);

                IReadOnlyList<GameObject> spawnedObjects =
                    spawnRuntime.Execute(
                        spawnRequest,
                        rewardSpawnConfig,
                        context.References.Runtime,
                        context.State);

                List<GameObject> lootObjects = new List<GameObject>();

                foreach (GameObject spawnedObject in spawnedObjects)
                {
                    if (spawnedObject == null)
                        continue;

                    lootObjects.Add(spawnedObject);
                }

                if (lootObjects.Count > 0)
                {
                    lootBurstPresentation.Play(
                        lootObjects.ToArray(),
                        resolvedLoot.Origin);
                }
            }

            reward.ConsumeResolvedLoot();
        }
    }
}