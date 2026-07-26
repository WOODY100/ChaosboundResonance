using Chaosbound.Shared.Identifiers;

namespace Chaosbound.Content.Expeditions.Definitions.Identity
{
    /// <summary>
    /// Represents the immutable identity of an expedition.
    /// </summary>
    public sealed class IdentityDefinition
    {
        public ContentId Id { get; }

        public IdentityDefinition(ContentId id)
        {
            Id = id;
        }
    }
}