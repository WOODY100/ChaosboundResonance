public sealed class EventConfig
{
    /// <summary>
    /// Represents an empty runtime configuration.
    /// </summary>
    public static EventConfig Empty { get; } =
        new EventConfig();

    private EventConfig()
    {
    }
}