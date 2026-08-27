using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Coordinates;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Viewport
{
    /// <summary>
    /// Controls the minimap viewport presentation.
    ///
    /// The player remains visually centered while the map content
    /// moves underneath it according to the player's world position.
    ///
    /// This class does not own world state or gameplay logic.
    /// </summary>
    public sealed class MinimapViewportController
    {
        private readonly RectTransform viewport;
        private readonly RectTransform mapContent;
        private readonly float zoom;

        private MinimapCoordinateMapper coordinateMapper;

        public RectTransform Viewport =>
            viewport;

        public RectTransform MapContent =>
            mapContent;

        public float Zoom =>
            zoom;

        public MinimapViewportController(
            RectTransform viewport,
            RectTransform mapContent,
            float zoom)
        {
            this.viewport =
                viewport
                ?? throw new ArgumentNullException(
                    nameof(viewport));

            this.mapContent =
                mapContent
                ?? throw new ArgumentNullException(
                    nameof(mapContent));

            if (zoom <= 0f)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(zoom));
            }

            this.zoom =
                zoom;
        }

        public void Initialize(
            Bounds worldBounds)
        {
            coordinateMapper =
                new MinimapCoordinateMapper(
                    worldBounds);

            ApplyZoom();
        }

        public void ApplyZoom()
        {
            mapContent.localScale =
                Vector3.one * zoom;
        }

        public void Update(
    Vector3 playerWorldPosition)
        {
            if (coordinateMapper == null)
                return;

            Vector2 normalized =
                coordinateMapper.WorldToNormalized(
                    playerWorldPosition);

            Vector2 viewportSize =
                viewport.rect.size;

            Vector2 mapSize =
                mapContent.rect.size;

            Vector2 scaledMapSize =
                mapSize * zoom;

            float maxOffsetX =
                Mathf.Max(
                    0f,
                    (scaledMapSize.x -
                     viewportSize.x) * 0.5f);

            float maxOffsetY =
                Mathf.Max(
                    0f,
                    (scaledMapSize.y -
                     viewportSize.y) * 0.5f);

            float offsetX =
                (0.5f - normalized.x) *
                scaledMapSize.x;

            float offsetY =
                (0.5f - normalized.y) *
                scaledMapSize.y;

            float unclampedOffsetX =
                offsetX;

            float unclampedOffsetY =
                offsetY;

            offsetX =
                Mathf.Clamp(
                    offsetX,
                    -maxOffsetX,
                    maxOffsetX);

            offsetY =
                Mathf.Clamp(
                    offsetY,
                    -maxOffsetY,
                    maxOffsetY);

            mapContent.anchoredPosition =
                new Vector2(
                    offsetX,
                    offsetY);
        }
    }
}