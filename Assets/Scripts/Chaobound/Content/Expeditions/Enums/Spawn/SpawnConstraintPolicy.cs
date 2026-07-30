namespace Chaosbound.Content.Expeditions.Enums.Spawn
{
    /// <summary>
    /// Describes a spawn restriction that may be applied
    /// during an expedition.
    /// </summary>
    public enum SpawnConstraintPolicy
    {
        CombatOnly = 0,

        MaximumAlive = 1,

        RequireFreeSpace = 2,

        RequireDifficulty = 3,

        DoorClosed = 4
    }
}