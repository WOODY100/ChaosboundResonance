namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers
{
    /// <summary>
    /// Defines how repeated applications of the same modifier definition behave.
    /// </summary>
    public enum ExpeditionModifierStackingPolicy
    {
        RefreshDuration,
        StackMagnitudeAndDuration,
        Independent
    }
}
