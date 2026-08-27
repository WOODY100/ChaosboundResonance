using System;
using UnityEngine;

namespace Chaosbound.Content.World.Themes.TileSets
{
    /// <summary>
    /// Declarative 4x4 cartographic mask for a world tile.
    /// A blocked cell is represented as true; false means walkable.
    /// The mask is local to the canonical orientation of the TileEntry.
    /// Runtime systems are responsible for applying the actual tile rotation.
    /// </summary>
    [Serializable]
    public sealed class MinimapTileMask
    {
        public const int Resolution = 4;
        public const int CellCount = Resolution * Resolution;

        [SerializeField]
        private bool[] blockedCells = new bool[CellCount];

        public bool IsBlocked(int x, int z)
        {
            ValidateCoordinates(x, z);
            EnsureStorage();
            return blockedCells[(z * Resolution) + x];
        }

        public void SetBlocked(int x, int z, bool blocked)
        {
            ValidateCoordinates(x, z);
            EnsureStorage();
            blockedCells[(z * Resolution) + x] = blocked;
        }

        public void Clear()
        {
            EnsureStorage();
            Array.Clear(blockedCells, 0, blockedCells.Length);
        }

        public void Fill()
        {
            EnsureStorage();
            for (int i = 0; i < blockedCells.Length; i++)
                blockedCells[i] = true;
        }

        public bool[] GetCopy()
        {
            EnsureStorage();
            return (bool[])blockedCells.Clone();
        }

        public void EnsureStorage()
        {
            if (blockedCells == null || blockedCells.Length != CellCount)
            {
                bool[] repaired = new bool[CellCount];

                if (blockedCells != null)
                {
                    int copyCount = Mathf.Min(
                        blockedCells.Length,
                        repaired.Length);

                    Array.Copy(
                        blockedCells,
                        repaired,
                        copyCount);
                }

                blockedCells = repaired;
            }
        }

        private static void ValidateCoordinates(int x, int z)
        {
            if (x < 0 || x >= Resolution)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (z < 0 || z >= Resolution)
                throw new ArgumentOutOfRangeException(nameof(z));
        }
    }
}
