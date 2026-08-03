using System;
using Chaosbound.Gameplay.Spawn.Placement.ValueObjects;

namespace Chaosbound.Gameplay.Spawn.Placement.Models
{
    /// <summary>
    /// Represents the immutable result of a placement
    /// resolution operation.
    /// </summary>
    public sealed class PlacementResolution
    {
        private readonly SpawnPlacement placement;

        private readonly FailureReason failureReason;

        /// <summary>
        /// Gets whether the placement resolution succeeded.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the resolved placement.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the resolution represents a failure.
        /// </exception>
        public SpawnPlacement Placement
        {
            get
            {
                if (!IsSuccess)
                {
                    throw new InvalidOperationException(
                        "Cannot access Placement from a failed PlacementResolution.");
                }

                return placement;
            }
        }

        /// <summary>
        /// Gets the reason why the placement failed.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the resolution represents a success.
        /// </exception>
        public FailureReason FailureReason
        {
            get
            {
                if (IsSuccess)
                {
                    throw new InvalidOperationException(
                        "Cannot access FailureReason from a successful PlacementResolution.");
                }

                return failureReason;
            }
        }

        /// <summary>
        /// Creates a successful placement resolution.
        /// </summary>
        public static PlacementResolution Success(
            SpawnPlacement placement)
        {
            return new PlacementResolution(
                true,
                placement,
                default);
        }

        /// <summary>
        /// Creates a failed placement resolution.
        /// </summary>
        public static PlacementResolution Failure(
            FailureReason failureReason)
        {
            return new PlacementResolution(
                false,
                default,
                failureReason);
        }

        private PlacementResolution(
            bool isSuccess,
            SpawnPlacement placement,
            FailureReason failureReason)
        {
            if (isSuccess)
            {
                if (placement == null)
                {
                    throw new ArgumentNullException(
                        nameof(placement));
                }
            }
            else
            {
                if (failureReason.Equals(default(FailureReason)))
                {
                    throw new ArgumentException(
                        "FailureReason must be provided.",
                        nameof(failureReason));
                }
            }

            IsSuccess = isSuccess;

            this.placement = placement;

            this.failureReason = failureReason;
        }
    }
}