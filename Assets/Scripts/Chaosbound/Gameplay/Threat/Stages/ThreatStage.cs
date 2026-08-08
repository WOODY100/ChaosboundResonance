using System;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using Chaosbound.Gameplay.Threat.Runtime;
using Chaosbound.Gameplay.Threat.Services;
using Chaosbound.Gameplay.Threat.ValueObjects;
using UnityEngine;

namespace Chaosbound.Gameplay.Threat.Stages
{
    /// <summary>
    /// Evaluates and updates the expedition threat budget.
    /// </summary>
    public sealed class ThreatStage :
        IExpeditionRuntimeStage
    {
        /// <inheritdoc/>
        public bool ShouldExecute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            return true;
        }

        /// <inheritdoc/>
        public void Execute(
            ExpeditionRuntimeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            ThreatCapacity capacity =
                ThreatBudgetEvaluator.Evaluate(
                    context.Config.Threat.BudgetPolicy,
                    context.State.CurrentPressure);

            Debug.Log(
                $"Pressure={context.State.CurrentPressure.Value}  EvaluatedCapacity={capacity.Value}");

            ThreatBudget budget =
                context.State.ThreatBudget;

            if (budget == null)
            {
                budget = new ThreatBudget(
                    capacity);

                context.State.SetThreatBudget(
                    budget);
            }
            else
            {
                budget.UpdateCapacity(
                    capacity);
            }
        }
    }
}