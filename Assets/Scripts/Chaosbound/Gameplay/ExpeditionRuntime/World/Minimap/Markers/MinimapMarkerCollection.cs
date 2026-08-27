using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Markers
{
    /// <summary>
    /// Runtime collection of minimap markers.
    /// </summary>
    public sealed class MinimapMarkerCollection
    {
        private readonly Dictionary<int, MinimapMarkerData> markers =
            new Dictionary<int, MinimapMarkerData>();

        public int Count =>
            markers.Count;

        public IEnumerable<MinimapMarkerData> Items =>
            markers.Values;

        public void Add(
            MinimapMarkerData marker)
        {
            if (marker == null)
            {
                throw new ArgumentNullException(
                    nameof(marker));
            }

            if (markers.ContainsKey(marker.Id))
            {
                throw new InvalidOperationException(
                    $"A minimap marker with ID {marker.Id} already exists.");
            }

            markers.Add(
                marker.Id,
                marker);
        }

        public bool Remove(
            int id)
        {
            return markers.Remove(id);
        }

        public bool TryGet(
            int id,
            out MinimapMarkerData marker)
        {
            return markers.TryGetValue(
                id,
                out marker);
        }

        public void Clear()
        {
            markers.Clear();
        }
    }
}