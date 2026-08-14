using Chaosbound.Content.Expeditions.Definitions.Timeline;
using System;

namespace Chaosbound.Content.Expeditions.Runtime.Timeline
{
    /// <summary>
    /// Immutable runtime configuration for the expedition timeline.
    /// </summary>
    public sealed class RuntimeTimelineConfig
    {
        /// <summary>
        /// Gets the materialized timeline agenda.
        /// </summary>
        public TimelineAgenda Agenda { get; }

        public RuntimeTimelineConfig(
            TimelineAgenda agenda)
        {
            Agenda =
                agenda
                ?? throw new ArgumentNullException(
                    nameof(agenda));
        }
    }
}