using Chaosbound.Shared.Contracts;

namespace Chaosbound.Shared.Contracts
{
    /// <summary>
    /// Represents a strongly typed reference to materializable content.
    /// </summary>
    /// <typeparam name="TIdentity">
    /// The strongly typed identity of the referenced content.
    /// </typeparam>
    public interface IMaterializableReference<TIdentity>
        where TIdentity : IIdentity
    {
        /// <summary>
        /// Gets the identity of the referenced content.
        /// </summary>
        TIdentity Identity { get; }
    }
}