using System;

namespace Chaosbound.Content.Expeditions.Definitions.Timeline
{
    public sealed class ExpeditionCompletionTarget
    {
        public float TimeSeconds { get; }

        public ExpeditionCompletionTarget(float timeSeconds)
        {
            if (timeSeconds <= 0f)
                throw new ArgumentOutOfRangeException(
                    nameof(timeSeconds),
                    "Completion target time must be greater than zero.");

            TimeSeconds = timeSeconds;
        }
    }
}