using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Contracts;
using UnityEngine;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Cleanup.Stages
{
    /// <summary>
    /// Cleans all runtime player skills and their
    /// materialized execution objects.
    /// </summary>
    public sealed class SkillCleanupStage :
        IExpeditionCleanupStage
    {
        public void Execute(
            ExpeditionCleanupContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            if (EnemyManager.Instance == null)
                return;

            Transform player =
                EnemyManager.Instance.Player;

            if (player == null)
                return;

            PlayerSkillExecutorSystem skillExecutorSystem =
                player.GetComponent<PlayerSkillExecutorSystem>();

            if (skillExecutorSystem == null)
                return;

            skillExecutorSystem.Cleanup();
        }
    }
}