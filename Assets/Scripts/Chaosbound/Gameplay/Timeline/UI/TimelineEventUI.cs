using Chaosbound.Content.Expeditions.Definitions.Timeline;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.UI.Timeline
{
    /// <summary>
    /// Visual representation of a single TimelineEntry.
    /// </summary>
    public sealed class TimelineEventUI : MonoBehaviour
    {
        [SerializeField]
        private Image m_Icon;

        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField]
        private float m_ReachedAlpha = 0.45f;

        [SerializeField]
        private float m_PendingAlpha = 1f;

        private TimelineEntry entry;

        /// <summary>
        /// Gets the timeline entry represented by this UI element.
        /// </summary>
        public TimelineEntry Entry =>
            entry;

        /// <summary>
        /// Gets the scheduled time of the represented entry.
        /// </summary>
        public float ScheduledTime =>
            entry != null
                ? entry.ScheduledTime
                : 0f;

        /// <summary>
        /// Gets the icon identity assigned to this entry.
        /// </summary>
        public string IconId =>
            entry?.IconId;

        /// <summary>
        /// Assigns the TimelineEntry represented by this UI.
        /// </summary>
        public void SetEntry(
            TimelineEntry timelineEntry)
        {
            if (timelineEntry == null)
                throw new ArgumentNullException(
                    nameof(timelineEntry));

            entry = timelineEntry;

            SetReached(false);

            // Icon resolution is intentionally deferred.
            // IconId is declarative content data.
        }

        /// <summary>
        /// Updates the visual reached/pending state.
        /// </summary>
        public void SetReached(
            bool reached)
        {
            if (m_CanvasGroup == null)
                return;

            m_CanvasGroup.alpha =
                reached
                    ? m_ReachedAlpha
                    : m_PendingAlpha;
        }

        /// <summary>
        /// Assigns the visual sprite to this event.
        /// </summary>
        public void SetIcon(
            Sprite sprite)
        {
            if (m_Icon == null)
                return;

            m_Icon.sprite = sprite;
        }
    }
}