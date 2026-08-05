using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Time.Contracts
{
    /// <summary>
    /// Provides the elapsed time for the current
    /// runtime tick.
    /// </summary>
    public interface ITimeProvider
    {
        /// <summary>
        /// Gets the elapsed time since the previous tick.
        /// </summary>
        TimeSpan DeltaTime
        {
            get;
        }
    }
}