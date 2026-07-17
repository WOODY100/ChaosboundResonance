public sealed class PlayerRunConfig
{
    /// <summary>
    /// Represents an empty runtime configuration.
    /// </summary>
    public static PlayerRunConfig Empty { get; } =
        new PlayerRunConfig();

    private PlayerRunConfig()
    {
    }
}