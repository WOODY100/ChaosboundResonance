using System;
using Chaosbound.Gameplay.Pressure.Profiles;

namespace Chaosbound.Content.Expeditions.Runtime.Pressure
{
    /// <summary>
    /// Runtime configuration for expedition pressure.
    /// </summary>
    public sealed class RuntimePressureConfig
    {
        public PressureCurveProfile CurveProfile { get; }

        public RuntimePressureConfig(
            PressureCurveProfile curveProfile)
        {
            CurveProfile = curveProfile
                ?? throw new ArgumentNullException(nameof(curveProfile));
        }
    }
}