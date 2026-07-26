using System;

namespace Chaosbound.Core.Runtime.State
{
    /// <summary>
    /// Represents the mutable runtime state of the current expedition.
    /// </summary>
    public sealed class RuntimeState
    {
        /// <summary>
        /// Elapsed expedition time in seconds.
        /// </summary>
        public float ElapsedTime { get; private set; }

        /// <summary>
        /// Advances the runtime timer.
        /// </summary>
        /// <param name="deltaTime">Elapsed time in seconds.</param>
        public void Advance(float deltaTime)
        {
            if (deltaTime < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaTime));
            }

            ElapsedTime += deltaTime;
        }

        /// <summary>
        /// Resets the runtime state.
        /// </summary>
        public void Reset()
        {
            ElapsedTime = 0f;
        }
    }
}