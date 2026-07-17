using System;

namespace Chaosbound.Runtime.Population
{
    /// <summary>
    /// Immutable snapshot describing the current runtime state
    /// required by the Population Director.
    /// </summary>
    public sealed class PopulationContext
    {
        /// <summary>
        /// Elapsed expedition time in seconds.
        /// </summary>
        public float ElapsedTime { get; }

        public PopulationContext(float elapsedTime)
        {
            if (elapsedTime < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(elapsedTime));
            }

            ElapsedTime = elapsedTime;
        }
    }
}