using Chaosbound.Gameplay.Pressure.ValueObjects;

namespace Chaosbound.Gameplay.Pressure.Models
{
    /// <summary>
    /// Represents an immutable snapshot of the
    /// current pressure state.
    /// </summary>
    public sealed class PressureSnapshot
    {
        /// <summary>
        /// Gets the evaluated pressure.
        /// </summary>
        public PressureValue Pressure
        {
            get;
        }

        public PressureSnapshot(
            PressureValue pressure)
        {
            Pressure = pressure;
        }
    }
}