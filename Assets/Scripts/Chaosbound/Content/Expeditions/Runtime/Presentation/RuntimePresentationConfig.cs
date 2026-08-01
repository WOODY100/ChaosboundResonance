namespace Chaosbound.Content.Expeditions.Runtime.Presentation
{
    /// <summary>
    /// Runtime representation of expedition presentation.
    /// </summary>
    public sealed class RuntimePresentationConfig
    {
        public string DisplayName { get; }

        public string Description { get; }

        public string IconId { get; }

        public RuntimePresentationConfig(
            string displayName,
            string description,
            string iconId)
        {
            DisplayName = displayName;
            Description = description;
            IconId = iconId;
        }
    }
}