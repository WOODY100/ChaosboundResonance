using Chaosbound.Shared.Authoring;
using Chaosbound.Shared.Identifiers;
using System;

namespace Chaosbound.Shared.Builders
{
    public static class ContentReferenceBuilder
    {
        public static ContentReference Build(
            ContentReferenceAuthoring authoring)
        {
            if (authoring == null)
                throw new ArgumentNullException(nameof(authoring));

            return new ContentReference(
                new ContentId(authoring.Id),
                authoring.Category);
        }
    }
}