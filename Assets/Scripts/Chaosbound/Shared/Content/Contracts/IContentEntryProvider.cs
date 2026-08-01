using Chaosbound.Shared.Identifiers;
using System.Collections.Generic;

namespace Chaosbound.Shared.Content.Contracts
{
    /// <summary>
    /// Exposes the content entries owned by a Definition.
    /// </summary>
    public interface IContentEntryProvider
    {
        IReadOnlyCollection<ContentReference> GetContentEntries();
    }
}