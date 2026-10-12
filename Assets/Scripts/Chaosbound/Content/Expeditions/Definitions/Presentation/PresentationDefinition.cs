
using System;
using UnityEngine;

namespace Chaosbound.Content.Expeditions.Definitions.Presentation
{
    /// <summary>
    /// Represents the immutable visual and textual presentation
    /// of an expedition. This data is intended for UI and player-facing
    /// systems only and contains no gameplay logic.
    /// </summary>
    public sealed class PresentationDefinition
    {
        public string DisplayName { get; }

        public string Description { get; }

        public Sprite Image { get; }

        public PresentationDefinition(
            string displayName,
            string description,
            Sprite image)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException(
                    "Display name cannot be null or empty.",
                    nameof(displayName));

            DisplayName = displayName;
            Description = description ?? string.Empty;
            Image = image;
        }
    }
}
