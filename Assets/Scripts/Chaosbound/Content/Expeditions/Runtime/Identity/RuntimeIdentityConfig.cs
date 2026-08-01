using Chaosbound.Shared.Identifiers;

namespace Chaosbound.Content.Expeditions.Runtime.Identity
{
    /// <summary>
    /// Runtime representation of expedition identity.
    /// </summary>
    public sealed class RuntimeIdentityConfig
    {
        public ContentId Id { get; }

        public RuntimeIdentityConfig(ContentId id)
        {
            Id = id;
        }
    }
}