using System;

namespace Chaosbound.Gameplay.Spawn.Content
{
    /// <summary>
    /// Resolves materializable content definitions
    /// by their ContentId.
    /// </summary>
    public sealed class MaterializableContentResolver
    {
        private readonly MaterializableContentDatabase
            database;

        public MaterializableContentResolver(
            MaterializableContentDatabase database)
        {
            this.database =
                database
                ?? throw new ArgumentNullException(
                    nameof(database));
        }

        public MaterializableContentDefinition Resolve(
            string contentId)
        {
            if (string.IsNullOrWhiteSpace(contentId))
            {
                throw new ArgumentException(
                    "ContentId cannot be null, empty, or whitespace.",
                    nameof(contentId));
            }

            if (!database.TryGet(
                contentId,
                out MaterializableContentDefinition definition))
            {
                throw new InvalidOperationException(
                    $"No materializable content was found " +
                    $"for ContentId '{contentId}'.");
            }

            return definition;
        }
    }
}