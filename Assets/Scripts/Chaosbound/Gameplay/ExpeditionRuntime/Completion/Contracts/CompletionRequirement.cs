namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts
{
    /// <summary>
    /// Defines the event required to complete
    /// the current expedition.
    /// </summary>
    public readonly struct CompletionRequirement
    {
        public string DomainId { get; }

        public string EventId { get; }

        public CompletionRequirement(
            string domainId,
            string eventId)
        {
            DomainId = domainId;
            EventId = eventId;
        }
    }
}