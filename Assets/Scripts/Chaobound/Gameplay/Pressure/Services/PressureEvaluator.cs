using System;
using Chaosbound.Gameplay.Pressure.Profiles;
using Chaosbound.Gameplay.Pressure.ValueObjects;

namespace Chaosbound.Gameplay.Pressure.Services
{
    /// <summary>
    /// Evaluates the current pressure of an expedition.
    /// </summary>
    public static class PressureEvaluator
    {
        public static PressureValue Evaluate(
            PressureCurveProfile profile,
            float elapsedSeconds)
        {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));

            return profile.Evaluate(
                elapsedSeconds);
        }
    }
}