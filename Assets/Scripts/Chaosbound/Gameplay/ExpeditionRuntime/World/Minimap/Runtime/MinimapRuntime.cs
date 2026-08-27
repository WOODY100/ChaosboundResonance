using Chaosbound.Core.Domain.Spatial;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Builders;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Config;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Coordinates;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Data;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Rendering;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Viewport;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Runtime
{
    /// <summary>
    /// Coordinates the runtime construction and presentation
    /// of the expedition minimap.
    ///
    /// This class does not own world generation and does not
    /// participate in the Expedition Runtime Pipeline.
    ///
    /// It coordinates:
    /// - static map construction
    /// - map viewport movement
    /// - player marker presentation
    /// </summary>
    public sealed class MinimapRuntime
    {
        private readonly MinimapMapBuilder mapBuilder;
        private readonly MinimapStaticMapRenderer staticMapRenderer;
        private readonly MinimapStaticMapView staticMapView;

        private readonly MinimapViewportController viewportController;

        private MinimapCoordinateMapper coordinateMapper;

        private MinimapMapData mapData;

        private readonly MinimapMarkerCollection markerCollection;

        private MinimapPlayerMarkerController playerMarkerController;

        //==========================================================
        // Public Properties
        //==========================================================

        public MinimapCoordinateMapper CoordinateMapper =>
            coordinateMapper;

        public MinimapMarkerCollection Markers =>
            markerCollection;

        public MinimapMapData MapData =>
            mapData;

        //==========================================================
        // Constructor
        //==========================================================

        public MinimapRuntime(
            MinimapStaticMapView staticMapView,
            MinimapConfig config,
            RectTransform viewport,
            RectTransform mapContent)
        {
            this.staticMapView =
                staticMapView
                ?? throw new ArgumentNullException(
                    nameof(staticMapView));

            if (config == null)
            {
                throw new ArgumentNullException(
                    nameof(config));
            }

            if (viewport == null)
            {
                throw new ArgumentNullException(
                    nameof(viewport));
            }

            if (mapContent == null)
            {
                throw new ArgumentNullException(
                    nameof(mapContent));
            }

            mapBuilder =
                new MinimapMapBuilder();

            staticMapRenderer =
                new MinimapStaticMapRenderer(
                    config.TileSize,
                    config.PixelsPerCell,
                    config.WalkableColor,
                    config.BlockedColor);

            markerCollection =
                new MinimapMarkerCollection();

            viewportController =
                new MinimapViewportController(
                    viewport,
                    mapContent,
                    config.Zoom);
        }

        //==========================================================
        // Update
        //==========================================================

        public void Update(
            Vector3 playerWorldPosition)
        {
            viewportController.Update(
                playerWorldPosition);

            if (playerMarkerController != null)
            {
                playerMarkerController.Update();
            }
        }

        //==========================================================
        // Player Marker
        //==========================================================

        public void InitializePlayerMarker(
    Transform playerTransform,
    MinimapMarkerView playerMarkerView)
        {
            if (playerTransform == null)
            {
                throw new ArgumentNullException(
                    nameof(playerTransform));
            }

            if (playerMarkerView == null)
            {
                throw new ArgumentNullException(
                    nameof(playerMarkerView));
            }

            if (coordinateMapper == null)
            {
                throw new InvalidOperationException(
                    "Coordinate mapper must be initialized before the player marker.");
            }

            RectTransform markerParent =
                playerMarkerView.transform.parent
                as RectTransform;

            if (markerParent == null)
            {
                throw new InvalidOperationException(
                    "Player marker must have a RectTransform parent.");
            }

            if (markerParent != viewportController.Viewport)
            {
                throw new InvalidOperationException(
                    "Player marker must be a child of the minimap viewport.");
            }

            playerMarkerController =
                new MinimapPlayerMarkerController(
                    playerTransform,
                    coordinateMapper,
                    viewportController.Viewport,
                    viewportController.MapContent,
                    playerMarkerView,
                    viewportController.Zoom);

            playerMarkerController.Update();
        }

        //==========================================================
        // Build
        //==========================================================

        public void Build(
            WorldLayout layout,
            Bounds worldBounds)
        {
            if (layout == null)
            {
                throw new ArgumentNullException(
                    nameof(layout));
            }

            mapData =
                mapBuilder.Build(
                    layout,
                    worldBounds);

            coordinateMapper =
                new MinimapCoordinateMapper(
                    worldBounds);

            viewportController.Initialize(
                worldBounds);

            Texture2D texture =
                staticMapRenderer.Render(
                    mapData);

            staticMapView.SetTexture(
                texture);
        }

        //==========================================================
        // Clear
        //==========================================================

        public void Clear()
        {
            mapData =
                null;

            staticMapView.Clear();

            playerMarkerController =
                null;
        }
    }
}