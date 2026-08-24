namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts
{
    /// <summary>
    /// Represents the completion of a gameplay event
    /// produced by an expedition domain.
    /// </summary>
    public readonly struct EventCompleted
    {
        /// <summary>
        /// Gets the domain that produced the completion.
        /// </summary>
        public string DomainId { get; }

        /// <summary>
        /// Gets the identifier of the completed event.
        /// </summary>
        public string EventId { get; }

        /// <summary>
        /// Gets the semantic origin that produced
        /// the completed event.
        /// </summary>
        public CompletionOrigin Origin { get; }

        public EventCompleted(
            string domainId,
            string eventId,
            CompletionOrigin origin)
        {
            DomainId = domainId;
            EventId = eventId;
            Origin = origin;
        }
    }
}