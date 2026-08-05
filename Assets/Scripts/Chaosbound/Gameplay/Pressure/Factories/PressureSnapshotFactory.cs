using Chaosbound.Gameplay.Pressure.Models;
using Chaosbound.Gameplay.Pressure.ValueObjects;

namespace Chaosbound.Gameplay.Pressure.Factories
{
    /// <summary>
    /// Creates immutable pressure snapshots.
    /// </summary>
    public sealed class PressureSnapshotFactory
    {
        public PressureSnapshot Create(
            PressureValue pressure)
        {
            return new PressureSnapshot(
                pressure);
        }
    }
}