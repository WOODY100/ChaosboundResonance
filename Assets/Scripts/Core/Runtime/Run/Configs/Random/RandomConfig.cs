public sealed class RandomConfig
{
    /// <summary>
    /// Represents an empty runtime configuration.
    /// </summary>
    public static RandomConfig Empty { get; } =
        new RandomConfig();

    private RandomConfig()
    {
    }
}