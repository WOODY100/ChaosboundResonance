using System;
using Chaosbound.Gameplay.ExpeditionRuntime.Context;
using Chaosbound.Gameplay.ExpeditionRuntime.Contracts;
using Chaosbound.Gameplay.Pressure.Services;
using Chaosbound.Gameplay.Pressure.ValueObjects;
using UnityEngine;

namespace Chaosbound.Gameplay.Pressure.Stages
{
    /// <summary>
    /// Evaluates the current expedition pressure.
    /// </summary>
    public sealed class PressureStage :
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

            PressureValue pressure =
                PressureEvaluator.Evaluate(
                    context.Config.Pressure.CurveProfile,
                    (float)context.State.ElapsedTime.TotalSeconds);

            context.State.SetPressure(
                pressure);
        }
    }
}