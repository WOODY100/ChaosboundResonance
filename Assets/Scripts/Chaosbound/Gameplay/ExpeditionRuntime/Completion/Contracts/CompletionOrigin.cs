using Chaosbound.Shared.Identifiers;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts
{
    /// <summary>
    /// Identifies the semantic origin that produced
    /// the completion of the current expedition.
    /// </summary>
    public readonly struct CompletionOrigin
    {
        /// <summary>
        /// Gets the domain that owns the origin.
        /// </summary>
        public string DomainId { get; }

        /// <summary>
        /// Gets the identifier of the content
        /// that produced the origin.
        /// </summary>
        public ContentId ContentId { get; }

        public CompletionOrigin(
            string domainId,
            ContentId contentId)
        {
            DomainId =
                domainId;

            ContentId =
                contentId;
        }
    }
}