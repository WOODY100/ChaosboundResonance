using System;

namespace Chaosbound.Content.Expeditions.Definitions.Presentation
{
    /// <summary>
    /// Represents the immutable presentation data of an expedition.
    /// </summary>
    public sealed class PresentationDefinition
    {
        public string DisplayName { get; }

        public string Description { get; }

        public string IconId { get; }

        public PresentationDefinition(
            string displayName,
            string description,
            string iconId)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException(
                    "Display name cannot be null or empty.",
                    nameof(displayName));

            DisplayName = displayName;
            Description = description ?? string.Empty;
            IconId = iconId ?? string.Empty;
        }
    }
}