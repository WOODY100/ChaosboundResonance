using System;
using Chaosbound.Gameplay.Pressure.Profiles;

namespace Chaosbound.Content.Expeditions.Definitions.Pressure
{
    /// <summary>
    /// Describes the pressure configuration used by an expedition.
    /// Contains only declarative data.
    /// </summary>
    public sealed class PressureDefinition
    {
        public PressureCurveProfile CurveProfile { get; }

        public PressureDefinition(
            PressureCurveProfile curveProfile)
        {
            CurveProfile = curveProfile
                ?? throw new ArgumentNullException(nameof(curveProfile));
        }
    }
}