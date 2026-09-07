using Chaosbound.Content.Expeditions.Runtime.Spawn;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.Spawn.Contracts;
using Chaosbound.Gameplay.Spawn.Runtime;
using System;

public sealed class RewardDomainDirector
{
    private readonly LootGenerator
        lootGenerator;

    private readonly RewardSpawnRequestBuilder
        spawnRequestBuilder;

    private readonly SpawnRuntime
        spawnRuntime;

    public RewardDomainDirector(
        LootGenerator lootGenerator,
        RewardSpawnRequestBuilder spawnRequestBuilder,
        SpawnRuntime spawnRuntime)
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
                spawnRequestBuilder.Build(
                    resolvedLoot,
                    context.Config.Spawn);

            if (!spawnRequest.IsEmpty)
            {
                spawnRuntime.Execute(
                    spawnRequest,
                    spawnRequest.Context.SpawnConfig,
                    context.References.Runtime,
                    context.State);
            }

            reward.ConsumeResolvedLoot();
        }
    }
}