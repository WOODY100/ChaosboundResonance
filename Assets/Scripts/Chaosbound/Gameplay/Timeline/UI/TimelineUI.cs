using Chaosbound.Content.Expeditions.Definitions.Timeline;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.UI.Timeline
{
    /// <summary>
    /// Presents the expedition Timeline as a temporal
    /// progress bar.
    ///
    /// This component does not evaluate Timeline logic.
    /// It only presents data produced by the Timeline domain.
    /// </summary>
    public sealed class TimelineUI : MonoBehaviour
    {
        [Header("Timeline Bar")]

        [SerializeField]
        private Image m_Fill;

        [Header("Events")]

        [SerializeField]
        private RectTransform m_EventContainer;

        [SerializeField]
        private TimelineEventUI m_EventPrefab;

        private readonly List<TimelineEventUI>
            m_EventInstances = new();

        [Header("Icon Mapping")]

        [SerializeField]
        private List<TimelineIconEntry> m_IconEntries =
            new();

        private TimelineAgenda agenda;

        /// <summary>
        /// Assigns the Timeline agenda represented by this UI.
        /// </summary>
        public void SetAgenda(
            TimelineAgenda timelineAgenda)
        {
            if (timelineAgenda == null)
                throw new ArgumentNullException(
                    nameof(timelineAgenda));

            agenda = timelineAgenda;

            ClearEvents();

            foreach (TimelineEntry entry
                     in agenda.Entries)
            {
                CreateEvent(entry);
            }

            UpdateProgress(0f);
        }

        /// <summary>
        /// Updates the visual progress of the Timeline.
        /// </summary>
        public void UpdateProgress(
            float elapsedTime)
        {
            if (agenda == null)
                return;

            if (elapsedTime < 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(elapsedTime),
                    "Elapsed time cannot be negative.");
            }

            float completionTarget =
                agenda.CompletionTargetTime;

            if (completionTarget <= 0f)
            {
                m_Fill.fillAmount = 0f;
                return;
            }

            float normalizedProgress =
                elapsedTime /
                completionTarget;

            m_Fill.fillAmount =
                Mathf.Clamp01(
                    normalizedProgress);

            UpdateEventStates(
                elapsedTime);
        }

        private void CreateEvent(
            TimelineEntry entry)
        {
            if (m_EventPrefab == null)
            {
                throw new InvalidOperationException(
                    "TimelineUI requires a TimelineEventUI prefab.");
            }

            if (m_EventContainer == null)
            {
                throw new InvalidOperationException(
                    "TimelineUI requires an EventContainer.");
            }

            TimelineEventUI instance =
                Instantiate(
                    m_EventPrefab,
                    m_EventContainer);

            instance.transform.localScale =
                Vector3.one;

            instance.SetEntry(entry);

            Sprite icon =
                ResolveIcon(entry.IconId);

            instance.SetIcon(icon);

            m_EventInstances.Add(instance);

            PositionEvent(
                instance,
                entry);
        }

        private void PositionEvent(
            TimelineEventUI eventUI,
            TimelineEntry entry)
        {
            RectTransform eventRect =
                eventUI.GetComponent<RectTransform>();

            if (eventRect == null)
                return;

            float normalizedTime =
                entry.ScheduledTime /
                agenda.CompletionTargetTime;

            normalizedTime =
                Mathf.Clamp01(
                    normalizedTime);

            RectTransform container =
                m_EventContainer;

            float containerWidth =
                container.rect.width;

            float eventWidth =
                eventRect.rect.width;

            float x =
                Mathf.Lerp(
                    eventWidth * 0.5f,
                    containerWidth -
                    eventWidth * 0.5f,
                    normalizedTime);

            eventRect.anchorMin =
                new Vector2(
                    0f,
                    0.5f);

            eventRect.anchorMax =
                new Vector2(
                    0f,
                    0.5f);

            eventRect.pivot =
                new Vector2(
                    0.5f,
                    0.5f);

            eventRect.anchoredPosition =
                new Vector2(
                    x,
                    0f);
        }

        private void UpdateEventStates(
            float elapsedTime)
        {
            foreach (
                TimelineEventUI eventUI
                in m_EventInstances)
            {
                if (eventUI == null)
                    continue;

                bool reached =
                    elapsedTime >=
                    eventUI.ScheduledTime;

                eventUI.SetReached(
                    reached);
            }
        }

        private void ClearEvents()
        {
            foreach (
                TimelineEventUI eventUI
                in m_EventInstances)
            {
                if (eventUI != null)
                    Destroy(eventUI.gameObject);
            }

            m_EventInstances.Clear();
        }

        private void OnDestroy()
        {
            m_EventInstances.Clear();
        }

        private Sprite ResolveIcon(
            string iconId)
        {
            if (string.IsNullOrWhiteSpace(iconId))
                return null;

            foreach (TimelineIconEntry entry in m_IconEntries)
            {
                if (entry == null)
                    continue;

                if (string.Equals(
                    entry.IconId,
                    iconId,
                    StringComparison.Ordinal))
                {
                    return entry.Sprite;
                }
            }

            Debug.LogWarning(
                $"{nameof(TimelineUI)}: No sprite mapping was found " +
                $"for IconId '{iconId}'.",
                this);

            return null;
        }
    }
}