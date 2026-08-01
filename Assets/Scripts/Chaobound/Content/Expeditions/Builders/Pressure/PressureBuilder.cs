using System;
using Chaosbound.Content.Expeditions.Authoring.Pressure;
using Chaosbound.Content.Expeditions.Definitions.Pressure;

namespace Chaosbound.Content.Expeditions.Builders.Pressure
{
    /// <summary>
    /// Converts pressure authoring into its domain representation.
    /// </summary>
    public static class PressureBuilder
    {
        public static PressureDefinition Build(
            PressureAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            return new PressureDefinition(
                authoring.CurveProfile);
        }
    }
}