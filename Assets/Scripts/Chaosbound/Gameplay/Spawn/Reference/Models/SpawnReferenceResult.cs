using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Spawn.Reference.Models
{
    /// <summary>
    /// Represents the result of resolving
    /// a spawn reference.
    /// </summary>
    public sealed class SpawnReferenceResult
    {
        /// <summary>
        /// Gets whether the resolution succeeded.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the resolved transform.
        /// </summary>
        public Transform Reference { get; }

        /// <summary>
        /// Gets the failure reason when
        /// the resolution fails.
        /// </summary>
        public string FailureReason { get; }

        private SpawnReferenceResult(
            bool isSuccess,
            Transform reference,
            string failureReason)
        {
            IsSuccess = isSuccess;
            Reference = reference;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Creates a successful resolution.
        /// </summary>
        public static SpawnReferenceResult Success(
            Transform reference)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));

            return new SpawnReferenceResult(
                true,
                reference,
                null);
        }

        /// <summary>
        /// Creates a failed resolution.
        /// </summary>
        public static SpawnReferenceResult Failure(
            string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException(
                    "Failure reason cannot be empty.",
                    nameof(reason));

            return new SpawnReferenceResult(
                false,
                null,
                reason);
        }
    }
}