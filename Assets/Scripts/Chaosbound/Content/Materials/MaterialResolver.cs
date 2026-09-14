using System;

namespace Chaosbound.Content.Materials
{
    public sealed class MaterialResolver
    {
        private readonly MaterialDatabase database;

        public MaterialResolver(MaterialDatabase database)
        {
            this.database =
                database
                ?? throw new ArgumentNullException(
                    nameof(database));
        }

        public bool TryResolve(
            string contentId,
            out MaterialDefinition definition)
        {
            return database.TryGet(
                contentId,
                out definition);
        }
    }
}