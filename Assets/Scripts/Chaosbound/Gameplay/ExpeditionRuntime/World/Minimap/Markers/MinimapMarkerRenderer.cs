using Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Coordinates;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers
{
    /// <summary>
    /// Converts runtime minimap marker data into UI marker views.
    ///
    /// This class does not own gameplay state.
    /// </summary>
    public sealed class MinimapMarkerRenderer
    {
        private readonly MinimapCoordinateMapper coordinateMapper;
        private readonly RectTransform markerContainer;

        private readonly Dictionary<int, MinimapMarkerView> views =
            new Dictionary<int, MinimapMarkerView>();

        public MinimapMarkerRenderer(
            MinimapCoordinateMapper coordinateMapper,
            RectTransform markerContainer)
        {
            this.coordinateMapper =
                coordinateMapper
                ?? throw new ArgumentNullException(
                    nameof(coordinateMapper));

            this.markerContainer =
                markerContainer
                ?? throw new ArgumentNullException(
                    nameof(markerContainer));
        }

        public void Render(
            MinimapMarkerData marker,
            MinimapMarkerView view)
        {
            if (marker == null)
            {
                throw new ArgumentNullException(
                    nameof(marker));
            }

            if (view == null)
            {
                throw new ArgumentNullException(
                    nameof(view));
            }

            Vector2 normalizedPosition =
                coordinateMapper.WorldToNormalized(
                    marker.WorldPosition);

            Vector2 containerSize =
                markerContainer.rect.size;

            Vector2 anchoredPosition =
                new Vector2(
                    (normalizedPosition.x - 0.5f) *
                    containerSize.x,

                    (normalizedPosition.y - 0.5f) *
                    containerSize.y);

            view.SetPosition(
                anchoredPosition);

            view.SetVisible(
                marker.IsVisible);
        }

        public Vector2 GetNormalizedPosition(
            Vector3 worldPosition)
        {
            return coordinateMapper.WorldToNormalized(
                worldPosition);
        }

        public void RegisterView(
            int markerId,
            MinimapMarkerView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(
                    nameof(view));
            }

            views[markerId] =
                view;
        }

        public bool TryGetView(
            int markerId,
            out MinimapMarkerView view)
        {
            return views.TryGetValue(
                markerId,
                out view);
        }

        public bool RemoveView(
            int markerId)
        {
            if (!views.TryGetValue(
                    markerId,
                    out MinimapMarkerView view))
            {
                return false;
            }

            views.Remove(
                markerId);

            if (view != null)
            {
                UnityEngine.Object.Destroy(
                    view.gameObject);
            }

            return true;
        }

        public void Clear()
        {
            foreach (
                MinimapMarkerView view
                in views.Values)
            {
                if (view != null)
                {
                    UnityEngine.Object.Destroy(
                        view.gameObject);
                }
            }

            views.Clear();
        }
    }
}