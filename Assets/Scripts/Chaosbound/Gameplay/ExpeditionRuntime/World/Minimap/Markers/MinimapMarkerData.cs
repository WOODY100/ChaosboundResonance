using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers
{
    /// <summary>
    /// Runtime data describing a marker displayed on the minimap.
    ///
    /// This class contains marker state only.
    /// It does not reference UI objects or world gameplay components.
    /// </summary>
    public sealed class MinimapMarkerData
    {
        public int Id
        {
            get;
        }

        public MinimapMarkerType Type
        {
            get;
        }

        public Vector3 WorldPosition
        {
            get;
            private set;
        }

        public bool IsVisible
        {
            get;
            private set;
        }

        public MinimapMarkerData(
            int id,
            MinimapMarkerType type,
            Vector3 worldPosition,
            bool isVisible = true)
        {
            if (id < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id));
            }

            Id =
                id;

            Type =
                type;

            WorldPosition =
                worldPosition;

            IsVisible =
                isVisible;
        }

        //==========================================================
        // Runtime State
        //==========================================================

        /// <summary>
        /// Updates the world position represented by this marker.
        /// </summary>
        public void SetWorldPosition(
            Vector3 worldPosition)
        {
            WorldPosition =
                worldPosition;
        }

        /// <summary>
        /// Updates the visibility state of this marker.
        /// </summary>
        public void SetVisible(
            bool visible)
        {
            IsVisible =
                visible;
        }
    }
}