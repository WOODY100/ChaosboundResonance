using Chaosbound.Shared.Identifiers;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.References.Models
{
    /// <summary>
    /// Identifies a runtime reference by
    /// domain and content identity.
    /// </summary>
    public readonly struct RuntimeReferenceKey :
        IEquatable<RuntimeReferenceKey>
    {
        public string DomainId { get; }

        public ContentId ContentId { get; }

        public RuntimeReferenceKey(
            string domainId,
            ContentId contentId)
        {
            if (string.IsNullOrWhiteSpace(domainId))
            {
                throw new ArgumentException(
                    "DomainId cannot be empty.",
                    nameof(domainId));
            }

            DomainId =
                domainId;

            ContentId =
                contentId;
        }

        public bool Equals(
            RuntimeReferenceKey other)
        {
            return
                DomainId ==
                other.DomainId
                &&
                ContentId.Equals(
                    other.ContentId);
        }

        public override bool Equals(
            object obj)
        {
            return
                obj is RuntimeReferenceKey other
                &&
                Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    (DomainId != null
                        ? DomainId.GetHashCode()
                        : 0)
                    * 397
                    +
                    ContentId.GetHashCode();
            }
        }
    }
}