using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Interactions
{
    /// <summary>
    /// Stores the runtime usage state of one-use interactables
    /// during the current expedition.
    ///
    /// This is runtime-only state.
    /// It is not persistent and is cleared with the expedition.
    /// </summary>
    public sealed class ExpeditionInteractableUsageState
    {
        private readonly HashSet<string> usedContentIds;

        public ExpeditionInteractableUsageState()
        {
            usedContentIds =
                new HashSet<string>();
        }

        /// <summary>
        /// Determines whether the specified interactable
        /// has already been used during this expedition.
        /// </summary>
        public bool HasBeenUsed(
            string contentId)
        {
            ValidateContentId(contentId);

            return usedContentIds.Contains(
                contentId);
        }

        /// <summary>
        /// Marks the specified interactable as used.
        /// </summary>
        public void MarkUsed(
            string contentId)
        {
            ValidateContentId(contentId);

            usedContentIds.Add(
                contentId);
        }

        /// <summary>
        /// Clears all interactable usage state.
        /// </summary>
        public void Clear()
        {
            usedContentIds.Clear();
        }

        private static void ValidateContentId(
            string contentId)
        {
            if (string.IsNullOrWhiteSpace(contentId))
            {
                throw new System.ArgumentException(
                    "ContentId cannot be empty.",
                    nameof(contentId));
            }
        }
    }
}