namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Represents the tactical functions that an enemy can contribute
    /// to the overall enemy composition.
    /// </summary>
    public enum TacticalCapability
    {
        MeleePressure = 0,
        RangedPressure = 1,
        AreaControl = 2,
        Mobility = 3,
        Absorption = 4,
        Reinforcement = 5
    }
}