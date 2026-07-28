using Chaosbound.Shared.Identifiers;

namespace Chaosbound.Shared.Content.Registry
{
    public interface IContentRegistry
    {
        bool TryGetAsset(
            ContentId id,
            out UnityEngine.Object asset);
    }
}