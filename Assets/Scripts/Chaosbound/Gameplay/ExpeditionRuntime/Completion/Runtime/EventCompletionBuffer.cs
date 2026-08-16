using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Runtime
{
    /// <summary>
    /// Stores EventCompleted notifications produced
    /// during the current Expedition Runtime tick.
    /// </summary>
    public sealed class EventCompletionBuffer
    {
        private readonly List<EventCompleted>
            events = new List<EventCompleted>(4);

        /// <summary>
        /// Gets the number of completion events currently buffered.
        /// </summary>
        public int Count =>
            events.Count;

        /// <summary>
        /// Adds a completed event to the current tick.
        /// </summary>
        public void Add(
            EventCompleted completedEvent)
        {
            events.Add(completedEvent);
        }

        /// <summary>
        /// Gets the events produced during the current tick.
        /// </summary>
        public IReadOnlyList<EventCompleted> Events =>
            events;

        /// <summary>
        /// Clears all completion events from the current tick.
        /// </summary>
        public void Clear()
        {
            events.Clear();
        }
    }
}