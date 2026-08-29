using System;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.World.Minimap.Coordinates
{
    /// <summary>
    /// Defines the orthonormal basis used by the minimap
    /// to project world positions into minimap space.
    ///
    /// The V1 minimap uses a fixed North-Up orientation:
    ///
    /// Right = world +X
    /// Up    = world +Z
    ///
    /// This type contains spatial data only.
    /// It does not know about cameras or UI.
    /// </summary>
    public readonly struct MinimapOrientationBasis
    {
        public Vector2 Right { get; }

        public Vector2 Up { get; }

        /// <summary>
        /// Gets the fixed North-Up minimap basis.
        /// </summary>
        public static MinimapOrientationBasis NorthUp =>
            new MinimapOrientationBasis(
                Vector2.right,
                Vector2.up);

        public MinimapOrientationBasis(
            Vector2 right,
            Vector2 up)
        {
            if (right.sqrMagnitude <= Mathf.Epsilon)
            {
                throw new ArgumentException(
                    "Minimap right axis must have a non-zero magnitude.",
                    nameof(right));
            }

            if (up.sqrMagnitude <= Mathf.Epsilon)
            {
                throw new ArgumentException(
                    "Minimap up axis must have a non-zero magnitude.",
                    nameof(up));
            }

            Right =
                right.normalized;

            Up =
                up.normalized;
        }
    }
}