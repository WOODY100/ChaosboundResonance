using System;

using Chaosbound.Content.Expeditions.Authoring.Identity;
using Chaosbound.Content.Expeditions.Definitions.Identity;
using Chaosbound.Shared.Identifiers;

namespace Chaosbound.Content.Expeditions.Builders.Identity
{
    /// <summary>
    /// Converts identity authoring data into its domain representation.
    /// </summary>
    public static class IdentityBuilder
    {
        public static IdentityDefinition Build(
            IdentityAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            return new IdentityDefinition(
                new ContentId(authoring.Id));
        }
    }
}