using Chaosbound.Content.Portal.Exit;

namespace Chaosbound.Content.Expeditions.Definitions.Completion
{
    /// <summary>
    /// Declarative completion requirement for an expedition.
    /// </summary>
    public sealed class CompletionDefinition
    {
        public string DomainId
        {
            get;
        }

        public string EventId
        {
            get;
        }

        public ExitPortalData ExitPortal
        {
            get;
        }

        public CompletionDefinition(
            string domainId,
            string eventId,
            ExitPortalData exitPortal)
        {
            DomainId =
                domainId;

            EventId =
                eventId;

            ExitPortal =
                exitPortal;
        }
    }
}