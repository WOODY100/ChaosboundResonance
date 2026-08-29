using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Gameplay.Bosses;
using Chaosbound.Gameplay.ExpeditionRuntime.References.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Coordinates;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers
{
    /// <summary>
    /// Synchronizes the active Boss runtime state
    /// with its minimap marker.
    ///
    /// The Boss marker follows the Boss world position.
    /// When the Boss is outside the visible minimap viewport,
    /// the marker is clamped to the viewport edge and rotated
    /// to indicate the Boss direction.
    ///
    /// This class does not own Boss gameplay state.
    /// It only consumes the Boss runtime state and
    /// runtime world references.
    /// </summary>
    public sealed class MinimapBossMarkerController
    {
        private const string BossDomainId =
            "boss";

        private readonly BossRuntimeState bossState;
        private readonly IRuntimeReferenceRegistry runtimeReferences;

        private readonly MinimapCoordinateMapper coordinateMapper;
        private readonly RectTransform viewport;
        private readonly RectTransform mapContent;

        private readonly MinimapMarkerCollection markerCollection;
        private readonly MinimapMarkerView markerView;

        private readonly MinimapMarkerData marker;

        private Transform bossTransform;

        private bool markerRegistered;

        private BossDomainState lastState;

        public MinimapBossMarkerController(
            BossRuntimeState bossState,
            IRuntimeReferenceRegistry runtimeReferences,
            MinimapCoordinateMapper coordinateMapper,
            RectTransform viewport,
            RectTransform mapContent,
            MinimapMarkerCollection markerCollection,
            MinimapMarkerView markerView)
        {
            this.bossState =
                bossState
                ?? throw new ArgumentNullException(
                    nameof(bossState));

            this.runtimeReferences =
                runtimeReferences
                ?? throw new ArgumentNullException(
                    nameof(runtimeReferences));

            this.coordinateMapper =
                coordinateMapper
                ?? throw new ArgumentNullException(
                    nameof(coordinateMapper));

            this.viewport =
                viewport
                ?? throw new ArgumentNullException(
                    nameof(viewport));

            this.mapContent =
                mapContent
                ?? throw new ArgumentNullException(
                    nameof(mapContent));

            this.markerCollection =
                markerCollection
                ?? throw new ArgumentNullException(
                    nameof(markerCollection));

            this.markerView =
                markerView
                ?? throw new ArgumentNullException(
                    nameof(markerView));

            marker =
                new MinimapMarkerData(
                    1,
                    MinimapMarkerType.Boss,
                    Vector3.zero,
                    false);

            lastState =
                bossState.State;

            markerView.SetVisible(
                false);
        }

        //==========================================================
        // Update
        //==========================================================

        public void Update()
        {
            BossDomainState currentState =
                bossState.State;

            if (currentState != lastState)
            {
                HandleStateChanged(
                    currentState);

                lastState =
                    currentState;
            }

            if (currentState != BossDomainState.Active)
            {
                markerView.SetVisible(
                    false);

                return;
            }

            if (bossTransform == null)
            {
                TryResolveBossTransform();

                if (bossTransform == null)
                {
                    return;
                }
            }

            ShowMarker();

            UpdateMarkerPosition();
        }

        //==========================================================
        // State
        //==========================================================

        private void HandleStateChanged(
            BossDomainState state)
        {
            switch (state)
            {
                case BossDomainState.Starting:
                    TryResolveBossTransform();
                    break;

                case BossDomainState.Active:
                    TryResolveBossTransform();

                    if (bossTransform != null)
                    {
                        ShowMarker();
                    }

                    break;

                case BossDomainState.Completed:
                    HideMarker();
                    break;

                case BossDomainState.Inactive:
                    HideMarker();
                    break;
            }
        }

        //==========================================================
        // Boss Reference
        //==========================================================

        private void TryResolveBossTransform()
        {
            BossData selectedBoss =
                bossState.SelectedBoss;

            if (selectedBoss == null)
            {
                return;
            }

            if (!runtimeReferences.TryResolve(
                    BossDomainId,
                    selectedBoss.Id,
                    out Transform transform))
            {
                return;
            }

            bossTransform =
                transform;
        }

        //==========================================================
        // Marker Lifecycle
        //==========================================================

        private void ShowMarker()
        {
            if (!markerRegistered)
            {
                markerCollection.Add(
                    marker);

                markerRegistered =
                    true;
            }

            marker.SetVisible(
                true);

            markerView.SetUseOffscreenImage(
                false);

            markerView.SetRotation(
                0f);

            markerView.SetVisible(
                true);
        }

        private void HideMarker()
        {
            marker.SetVisible(
                false);

            markerView.SetVisible(
                false);

            markerView.SetUseOffscreenImage(
                false);

            markerView.SetRotation(
                0f);

            bossTransform =
                null;
        }

        //==========================================================
        // Position
        //==========================================================

        private void UpdateMarkerPosition()
        {
            Vector3 bossWorldPosition =
                bossTransform.position;

            marker.SetWorldPosition(
                bossWorldPosition);

            Vector2 normalizedPosition =
                coordinateMapper.WorldToNormalized(
                    bossWorldPosition);

            Vector2 mapSize =
                mapContent.rect.size;

            Vector2 scaledMapSize =
                mapSize *
                mapContent.localScale.x;

            Vector2 mapPosition =
                new Vector2(
                    (normalizedPosition.x - 0.5f) *
                    scaledMapSize.x,

                    (normalizedPosition.y - 0.5f) *
                    scaledMapSize.y);

            Vector2 viewportPosition =
                mapPosition +
                mapContent.anchoredPosition;

            Rect viewportRect =
                viewport.rect;

            bool insideViewport =
                viewportRect.Contains(
                    viewportPosition);

            if (insideViewport)
            {
                //======================================================
                // Boss dentro del minimapa.
                //======================================================

                markerView.SetUseOffscreenImage(
                    false);

                markerView.SetPosition(
                    viewportPosition);

                markerView.SetRotation(
                    0f);

                markerView.SetVisible(
                    true);

                return;
            }

            //==========================================================
            // Boss fuera del minimapa.
            //==========================================================

            Vector2 direction =
                viewportPosition;

            if (direction.sqrMagnitude <=
                Mathf.Epsilon)
            {
                return;
            }

            Vector2 clampedPosition =
                ClampToViewport(
                    direction,
                    viewportRect);

            // IMPORTANTE:
            //
            // clampedPosition ya está expresado en las coordenadas
            // locales del viewport.
            //
            // NO debemos sumar mapContent.anchoredPosition aquí.
            //
            markerView.SetPosition(
                clampedPosition);

            markerView.SetUseOffscreenImage(
                true);

            float angle =
                Mathf.Atan2(
                    direction.y,
                    direction.x) *
                Mathf.Rad2Deg;

            markerView.SetRotation(
                angle - 90f);

            markerView.SetVisible(
                true);
        }

        //==========================================================
        // Off-Screen Indicator
        //==========================================================

        private Vector2 ClampToViewport(
            Vector2 position,
            Rect viewportRect)
        {
            const float margin = 8f;

            float halfWidth =
                Mathf.Max(
                    0f,
                    viewportRect.width * 0.5f -
                    margin);

            float halfHeight =
                Mathf.Max(
                    0f,
                    viewportRect.height * 0.5f -
                    margin);

            if (halfWidth <= 0f ||
                halfHeight <= 0f)
            {
                return Vector2.zero;
            }

            float scaleX =
                halfWidth /
                Mathf.Abs(position.x);

            float scaleY =
                halfHeight /
                Mathf.Abs(position.y);

            float scale =
                Mathf.Min(
                    scaleX,
                    scaleY);

            if (scale > 1f)
            {
                scale = 1f;
            }

            return position * scale;
        }

        //==========================================================
        // Clear
        //==========================================================

        public void Clear()
        {
            marker.SetVisible(
                false);

            markerView.SetVisible(
                false);

            if (markerRegistered)
            {
                markerCollection.Remove(
                    marker.Id);

                markerRegistered =
                    false;
            }

            bossTransform =
                null;
        }
    }
}