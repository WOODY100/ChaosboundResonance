using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using System;

public sealed class RewardStage :
    IExpeditionRuntimeStage
{
    private readonly RewardDomainDirector
        rewardDomainDirector;

    public RewardStage(
        RewardDomainDirector rewardDomainDirector)
    {
        this.rewardDomainDirector =
            rewardDomainDirector
            ?? throw new ArgumentNullException(
                nameof(rewardDomainDirector));
    }

    public bool ShouldExecute(
        ExpeditionRuntimeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(
                nameof(context));
        }

        return true;
    }

    public void Execute(
        ExpeditionRuntimeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(
                nameof(context));
        }

        rewardDomainDirector.Execute(
            context);
    }
}