namespace Chaosbound.Content.Expeditions.Definitions.Completion
{
    /// <summary>
    /// Declarative completion requirement for an expedition.
    /// </summary>
    public sealed class CompletionDefinition
    {
        /// <summary>
        /// Gets the domain that must report completion.
        /// </summary>
        public string DomainId
        {
            get;
        }

        /// <summary>
        /// Gets the content event that must be completed.
        /// </summary>
        public string EventId
        {
            get;
        }

        public CompletionDefinition(
            string domainId,
            string eventId)
        {
            DomainId =
                domainId;

            EventId =
                eventId;
        }
    }
}