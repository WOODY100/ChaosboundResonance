using System;
using Unity;
using Chaosbound.Gameplay.ExpeditionRuntime.Time.Contracts;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Time.Providers
{
    /// <summary>
    /// Provides runtime time using Unity's Time API.
    /// </summary>
    public sealed class UnityTimeProvider :
        ITimeProvider
    {
        /// <inheritdoc/>
        public TimeSpan DeltaTime
        {
            get
            {
                return TimeSpan.FromSeconds(
                    UnityEngine.Time.deltaTime);
            }
        }
    }
}