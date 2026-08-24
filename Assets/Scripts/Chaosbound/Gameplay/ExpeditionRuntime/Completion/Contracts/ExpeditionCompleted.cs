namespace Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts
{
    /// <summary>
    /// Represents the successful completion of
    /// the current expedition.
    /// </summary>
    public readonly struct ExpeditionCompleted
    {
        /// <summary>
        /// Gets the semantic origin that produced
        /// the expedition completion.
        /// </summary>
        public CompletionOrigin Origin { get; }

        public ExpeditionCompleted(
            CompletionOrigin origin)
        {
            Origin =
                origin;
        }
    }
}