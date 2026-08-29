using Chaosbound.Content.Portal.Exit;
using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.References.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Coordinates;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers
{
    /// <summary>
    /// Synchronizes the Exit Portal runtime state
    /// with its minimap marker.
    ///
    /// The Exit Portal has a fixed world position once
    /// it has been materialized.
    ///
    /// When the portal is inside the visible minimap
    /// viewport, its normal marker is displayed.
    ///
    /// When the portal is outside the visible viewport,
    /// the marker is clamped to the viewport edge and
    /// uses the off-screen image to indicate its direction.
    ///
    /// This class does not own Exit Portal gameplay state.
    /// It only consumes runtime state and runtime references.
    /// </summary>
    public sealed class MinimapPortalMarkerController
    {
        private const string ExitPortalDomainId =
            "exitPortal";

        private readonly ExitPortalRuntimeState portalState;

        private readonly ExitPortalData portalData;

        private readonly IRuntimeReferenceRegistry runtimeReferences;

        private readonly MinimapCoordinateMapper coordinateMapper;

        private readonly RectTransform viewport;

        private readonly RectTransform mapContent;

        private readonly MinimapMarkerCollection markerCollection;

        private readonly MinimapMarkerView markerView;

        private readonly MinimapMarkerData marker;

        private Vector3 portalWorldPosition;

        private bool portalPositionResolved;

        private bool markerRegistered;

        private ExitPortalDomainState lastState;

        public MinimapPortalMarkerController(
            ExitPortalRuntimeState portalState,
            ExitPortalData portalData,
            IRuntimeReferenceRegistry runtimeReferences,
            MinimapCoordinateMapper coordinateMapper,
            RectTransform viewport,
            RectTransform mapContent,
            MinimapMarkerCollection markerCollection,
            MinimapMarkerView markerView)
        {
            this.portalState =
                portalState
                ?? throw new ArgumentNullException(
                    nameof(portalState));

            this.portalData =
                portalData
                ?? throw new ArgumentNullException(
                    nameof(portalData));

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
                    2,
                    MinimapMarkerType.ExitPortal,
                    Vector3.zero,
                    false);

            lastState =
                portalState.State;

            markerView.SetVisible(
                false);
        }

        //==========================================================
        // Update
        //==========================================================

        public void Update()
        {
            ExitPortalDomainState currentState =
                portalState.State;

            if (currentState != lastState)
            {
                HandleStateChanged(
                    currentState);

                lastState =
                    currentState;
            }

            if (currentState !=
                ExitPortalDomainState.Spawned)
            {
                markerView.SetVisible(
                    false);

                return;
            }

            if (!portalPositionResolved)
            {
                TryResolvePortalPosition();

                if (!portalPositionResolved)
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
            ExitPortalDomainState state)
        {
            switch (state)
            {
                case ExitPortalDomainState.Inactive:
                    HideMarker();
                    break;

                case ExitPortalDomainState.Waiting:
                    HideMarker();
                    break;

                case ExitPortalDomainState.Spawned:
                    TryResolvePortalPosition();

                    if (portalPositionResolved)
                    {
                        ShowMarker();
                    }

                    break;
            }
        }

        //==========================================================
        // Portal Reference
        //==========================================================

        private void TryResolvePortalPosition()
        {
            if (portalPositionResolved)
            {
                return;
            }

            if (!runtimeReferences.TryResolve(
                    ExitPortalDomainId,
                    portalData.Id,
                    out Transform transform))
            {
                return;
            }

            portalWorldPosition =
                transform.position;

            portalPositionResolved =
                true;
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
        }

        //==========================================================
        // Position
        //==========================================================

        private void UpdateMarkerPosition()
        {
            marker.SetWorldPosition(
                portalWorldPosition);

            Vector2 normalizedPosition =
                coordinateMapper.WorldToNormalized(
                    portalWorldPosition);

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

            markerView.SetUseOffscreenImage(
                false);

            markerView.SetRotation(
                0f);

            if (markerRegistered)
            {
                markerCollection.Remove(
                    marker.Id);

                markerRegistered =
                    false;
            }

            portalPositionResolved =
                false;

            portalWorldPosition =
                Vector3.zero;
        }
    }
}