using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Coordinates;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers
{
    /// <summary>
    /// Presents a fixed world marker inside the minimap.
    ///
    /// The marker represents a world position that does not
    /// change during runtime.
    ///
    /// When the marker is inside the visible minimap viewport,
    /// its normal image is displayed.
    ///
    /// When the marker leaves the visible viewport, it is hidden.
    ///
    /// This controller does not use an off-screen indicator.
    /// It does not clamp the marker to the viewport edge.
    ///
    /// This type is intentionally generic and can represent
    /// modifier structures, shrines, NPCs, treasures or other
    /// fixed world points of interest.
    /// </summary>
    public sealed class MinimapWorldMarkerController
    {
        private readonly MinimapCoordinateMapper coordinateMapper;

        private readonly RectTransform viewport;

        private readonly RectTransform mapContent;

        private readonly MinimapMarkerCollection markerCollection;

        private readonly MinimapMarkerView markerView;

        private readonly MinimapMarkerData marker;

        private readonly float zoom;

        private bool markerRegistered;

        public MinimapWorldMarkerController(
            MinimapMarkerData marker,
            MinimapCoordinateMapper coordinateMapper,
            RectTransform viewport,
            RectTransform mapContent,
            MinimapMarkerCollection markerCollection,
            MinimapMarkerView markerView,
            float zoom)
        {
            this.marker =
                marker
                ?? throw new ArgumentNullException(
                    nameof(marker));

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

            if (zoom <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(zoom));
            }

            this.zoom =
                zoom;

            markerView.SetVisible(
                false);
        }

        //==========================================================
        // Update
        //==========================================================

        public void Update()
        {
            if (!markerRegistered)
            {
                RegisterMarker();
            }

            if (!marker.IsVisible)
            {
                markerView.SetVisible(
                    false);

                return;
            }

            UpdateMarkerPosition();
        }

        //==========================================================
        // Marker Lifecycle
        //==========================================================

        private void RegisterMarker()
        {
            markerCollection.Add(
                marker);

            markerRegistered =
                true;
        }

        //==========================================================
        // Position
        //==========================================================

        private void UpdateMarkerPosition()
        {
            Vector2 normalizedPosition =
                coordinateMapper.WorldToNormalized(
                    marker.WorldPosition);

            Vector2 mapSize =
                mapContent.rect.size;

            Vector2 scaledMapSize =
                mapSize *
                zoom;

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

            if (!insideViewport)
            {
                markerView.SetVisible(
                    false);

                return;
            }

            markerView.SetUseOffscreenImage(
                false);

            markerView.SetRotation(
                0f);

            markerView.SetPosition(
                viewportPosition);

            markerView.SetVisible(
                true);
        }

        //==========================================================
        // Clear
        //==========================================================

        public void Clear()
        {
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
        }
    }
}