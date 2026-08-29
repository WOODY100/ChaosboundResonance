using Chaosbound.Content.Expeditions.Runtime.Minimap;
using Chaosbound.Content.Portal.Exit;
using Chaosbound.Gameplay.Bosses;
using Chaosbound.Gameplay.ExpeditionRuntime.ExitPortal.Runtime;
using Chaosbound.Gameplay.ExpeditionRuntime.References.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Builders;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Config;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Coordinates;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Data;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Rendering;
using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Viewport;
using System;
using System.Collections.Generic;
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
    /// - Boss marker presentation
    /// </summary>
    public sealed class MinimapRuntime
    {
        private readonly MinimapMapBuilder mapBuilder;
        private readonly MinimapStaticMapRenderer staticMapRenderer;
        private readonly MinimapStaticMapView staticMapView;

        private readonly MinimapViewportController viewportController;

        private readonly MinimapOrientationBasis orientationBasis;

        private MinimapCoordinateMapper coordinateMapper;

        private MinimapMapData mapData;

        private readonly MinimapMarkerCollection markerCollection;

        private MinimapPlayerMarkerController playerMarkerController;

        private MinimapBossMarkerController bossMarkerController;

        private MinimapPortalMarkerController portalMarkerController;

        private readonly List<MinimapWorldMarkerController>
            worldMarkerControllers =
                new List<MinimapWorldMarkerController>();

        private int nextWorldMarkerId =
            100000;

        private int GetNextWorldMarkerId()
        {
            while (
                markerCollection.TryGet(
                    nextWorldMarkerId,
                    out _))
            {
                nextWorldMarkerId++;
            }

            return nextWorldMarkerId++;
        }

        private readonly RuntimeMinimapConfig runtimeConfig;

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
            RuntimeMinimapConfig runtimeConfig,
            RectTransform viewport,
            RectTransform mapContent,
            MinimapOrientationBasis orientationBasis)
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

            this.runtimeConfig =
                runtimeConfig
                ?? throw new ArgumentNullException(
                    nameof(runtimeConfig));

            this.orientationBasis =
                orientationBasis;

            mapBuilder =
                new MinimapMapBuilder();

            staticMapRenderer =
                new MinimapStaticMapRenderer(
                    config.TileSize,
                    config.PixelsPerCell,
                    config.BlockedColor,
                    runtimeConfig.WalkableTexture,
                    orientationBasis);

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

            if (bossMarkerController != null)
            {
                bossMarkerController.Update();
            }

            if (portalMarkerController != null)
            {
                portalMarkerController.Update();
            }

            for (int i = 0;
                 i < worldMarkerControllers.Count;
                 i++)
            {
                worldMarkerControllers[i].Update();
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
        // Boss Marker
        //==========================================================

        public void InitializeBossMarker(
            BossRuntimeState bossState,
            IRuntimeReferenceRegistry runtimeReferences,
            MinimapMarkerView bossMarkerView)
        {
            if (bossState == null)
            {
                throw new ArgumentNullException(
                    nameof(bossState));
            }

            if (runtimeReferences == null)
            {
                throw new ArgumentNullException(
                    nameof(runtimeReferences));
            }

            if (bossMarkerView == null)
            {
                throw new ArgumentNullException(
                    nameof(bossMarkerView));
            }

            if (coordinateMapper == null)
            {
                throw new InvalidOperationException(
                    "Coordinate mapper must be initialized before the Boss marker.");
            }

            RectTransform markerParent =
                bossMarkerView.transform.parent
                as RectTransform;

            if (markerParent == null)
            {
                throw new InvalidOperationException(
                    "Boss marker must have a RectTransform parent.");
            }

            if (markerParent != viewportController.Viewport)
            {
                throw new InvalidOperationException(
                    "Boss marker must be a child of the minimap viewport.");
            }

            bossMarkerController =
                new MinimapBossMarkerController(
                    bossState,
                    runtimeReferences,
                    coordinateMapper,
                    viewportController.Viewport,
                    viewportController.MapContent,
                    markerCollection,
                    bossMarkerView);
        }

        //==========================================================
        // Exit Portal Marker
        //==========================================================

        public void InitializePortalMarker(
            ExitPortalRuntimeState portalState,
            ExitPortalData portalData,
            IRuntimeReferenceRegistry runtimeReferences,
            MinimapMarkerView portalMarkerView)
        {
            if (portalState == null)
            {
                throw new ArgumentNullException(
                    nameof(portalState));
            }

            if (portalData == null)
            {
                throw new ArgumentNullException(
                    nameof(portalData));
            }

            if (runtimeReferences == null)
            {
                throw new ArgumentNullException(
                    nameof(runtimeReferences));
            }

            if (portalMarkerView == null)
            {
                throw new ArgumentNullException(
                    nameof(portalMarkerView));
            }

            if (coordinateMapper == null)
            {
                throw new InvalidOperationException(
                    "Coordinate mapper must be initialized before the Exit Portal marker.");
            }

            RectTransform markerParent =
                portalMarkerView.transform.parent
                as RectTransform;

            if (markerParent == null)
            {
                throw new InvalidOperationException(
                    "Exit Portal marker must have a RectTransform parent.");
            }

            if (markerParent != viewportController.Viewport)
            {
                throw new InvalidOperationException(
                    "Exit Portal marker must be a child of the minimap viewport.");
            }

            portalMarkerController =
                new MinimapPortalMarkerController(
                    portalState,
                    portalData,
                    runtimeReferences,
                    coordinateMapper,
                    viewportController.Viewport,
                    viewportController.MapContent,
                    markerCollection,
                    portalMarkerView);
        }

        //==========================================================
        // Fixed World Markers
        //==========================================================

        public void InitializeWorldMarkers(
            IReadOnlyList<Vector3> worldPositions,
            MinimapMarkerView markerPrefab)
        {
            if (worldPositions == null)
            {
                throw new ArgumentNullException(
                    nameof(worldPositions));
            }

            if (markerPrefab == null)
            {
                throw new ArgumentNullException(
                    nameof(markerPrefab));
            }

            if (coordinateMapper == null)
            {
                throw new InvalidOperationException(
                    "Coordinate mapper must be initialized before world markers.");
            }

            RectTransform markerParent =
                viewportController.Viewport;

            if (markerParent == null)
            {
                throw new InvalidOperationException(
                    "Minimap viewport is missing.");
            }

            ClearWorldMarkers();

            for (int i = 0;
                 i < worldPositions.Count;
                 i++)
            {
                MinimapMarkerData marker =
                    new MinimapMarkerData(
                        GetNextWorldMarkerId(),
                        MinimapMarkerType.ModifierStructure,
                        worldPositions[i]);

                MinimapMarkerView markerView =
                    UnityEngine.Object.Instantiate(
                        markerPrefab,
                        markerParent);

                MinimapWorldMarkerController controller =
                    new MinimapWorldMarkerController(
                        marker,
                        coordinateMapper,
                        viewportController.Viewport,
                        viewportController.MapContent,
                        markerCollection,
                        markerView,
                        viewportController.Zoom);

                worldMarkerControllers.Add(
                    controller);

                controller.Update();
            }
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
                    worldBounds,
                    orientationBasis);

            viewportController.Initialize(
                coordinateMapper);

            Texture2D texture =
                staticMapRenderer.Render(
                    mapData);

            staticMapView.SetTexture(
                texture);
        }

        //==========================================================
        // Clear
        //==========================================================

        private void ClearWorldMarkers()
        {
            for (int i = 0;
                 i < worldMarkerControllers.Count;
                 i++)
            {
                worldMarkerControllers[i].Clear();
            }

            worldMarkerControllers.Clear();
        }

        public void Clear()
        {
            mapData =
                null;

            staticMapView.Clear();

            playerMarkerController =
                null;

            bossMarkerController?.Clear();

            bossMarkerController =
                null;

            portalMarkerController?.Clear();

            portalMarkerController =
                null;

            ClearWorldMarkers();

            nextWorldMarkerId =
                100000;

            coordinateMapper =
                null;
        }
    }
}