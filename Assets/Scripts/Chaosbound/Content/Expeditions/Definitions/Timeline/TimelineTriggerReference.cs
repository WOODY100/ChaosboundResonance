using System;

namespace Chaosbound.Content.Expeditions.Definitions.Timeline
{
    /// <summary>
    /// Declarative reference to content that must be interpreted
    /// by another domain when a timeline entry is reached.
    /// </summary>
    public sealed class TimelineTriggerReference
    {
        public string DomainId { get; }

        public string ContentId { get; }

        public TimelineTriggerReference(
            string domainId,
            string contentId)
        {
            if (string.IsNullOrWhiteSpace(domainId))
                throw new ArgumentException(
                    "Domain id cannot be null or empty.",
                    nameof(domainId));

            if (string.IsNullOrWhiteSpace(contentId))
                throw new ArgumentException(
                    "Content id cannot be null or empty.",
                    nameof(contentId));

            DomainId = domainId;
            ContentId = contentId;
        }
    }
}