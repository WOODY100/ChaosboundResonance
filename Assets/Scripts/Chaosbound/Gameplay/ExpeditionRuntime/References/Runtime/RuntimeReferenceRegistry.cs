using Chaosbound.Gameplay.ExpeditionRuntime.References.Contracts;
using Chaosbound.Gameplay.ExpeditionRuntime.References.Models;
using Chaosbound.Shared.Identifiers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.References.Runtime
{
    /// <summary>
    /// Stores runtime world references associated
    /// with the current expedition.
    /// </summary>
    public sealed class RuntimeReferenceRegistry :
        IRuntimeReferenceRegistry
    {
        private readonly Dictionary<
            RuntimeReferenceKey,
            Transform>
            references =
                new Dictionary<
                    RuntimeReferenceKey,
                    Transform>();

        /// <summary>
        /// Registers a runtime world reference.
        /// </summary>
        public void Register(
            string domainId,
            ContentId contentId,
            Transform transform)
        {
            if (string.IsNullOrWhiteSpace(domainId))
            {
                throw new ArgumentException(
                    "DomainId cannot be empty.",
                    nameof(domainId));
            }

            if (transform == null)
            {
                throw new ArgumentNullException(
                    nameof(transform));
            }

            RuntimeReferenceKey key =
                new RuntimeReferenceKey(
                    domainId,
                    contentId);

            references[key] =
                transform;
        }

        /// <summary>
        /// Removes a runtime world reference.
        /// </summary>
        public void Unregister(
            string domainId,
            ContentId contentId)
        {
            if (string.IsNullOrWhiteSpace(domainId))
            {
                throw new ArgumentException(
                    "DomainId cannot be empty.",
                    nameof(domainId));
            }

            RuntimeReferenceKey key =
                new RuntimeReferenceKey(
                    domainId,
                    contentId);

            references.Remove(
                key);
        }

        /// <summary>
        /// Attempts to resolve a runtime world reference.
        /// </summary>
        public bool TryResolve(
            string domainId,
            ContentId contentId,
            out Transform transform)
        {
            transform = null;

            if (string.IsNullOrWhiteSpace(domainId))
            {
                return false;
            }

            RuntimeReferenceKey key =
                new RuntimeReferenceKey(
                    domainId,
                    contentId);

            if (!references.TryGetValue(
                key,
                out Transform registeredTransform))
            {
                return false;
            }

            if (registeredTransform == null)
            {
                return false;
            }

            transform =
                registeredTransform;

            return true;
        }
    }
}