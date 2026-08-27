using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Coordinates;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers
{
    /// <summary>
    /// Synchronizes the player world position
    /// with the player marker displayed inside
    /// the minimap viewport.
    ///
    /// The marker remains centered while the map can
    /// follow the player. When the map reaches a world
    /// boundary, the marker moves inside the viewport
    /// to represent the player's actual visible position.
    /// </summary>
    public sealed class MinimapPlayerMarkerController
    {
        private readonly Transform playerTransform;
        private readonly MinimapCoordinateMapper coordinateMapper;
        private readonly RectTransform viewport;
        private readonly RectTransform mapContent;
        private readonly MinimapMarkerView markerView;

        private readonly float zoom;

        public MinimapPlayerMarkerController(
            Transform playerTransform,
            MinimapCoordinateMapper coordinateMapper,
            RectTransform viewport,
            RectTransform mapContent,
            MinimapMarkerView markerView,
            float zoom)
        {
            this.playerTransform =
                playerTransform
                ?? throw new ArgumentNullException(
                    nameof(playerTransform));

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
        }

        public void Update()
        {
            if (playerTransform == null)
                return;

            Vector2 normalized =
                coordinateMapper.WorldToNormalized(
                    playerTransform.position);

            Vector2 mapSize =
                mapContent.rect.size;

            Vector2 scaledMapSize =
                mapSize * zoom;

            Vector2 playerPositionOnMap =
                new Vector2(
                    (normalized.x - 0.5f) *
                    scaledMapSize.x,

                    (normalized.y - 0.5f) *
                    scaledMapSize.y);

            Vector2 markerPosition =
                mapContent.anchoredPosition +
                playerPositionOnMap;

            markerView.SetPosition(
                markerPosition);

            markerView.SetVisible(
                true);
        }
    }
}