namespace Chaosbound.Gameplay.Spawn.Contracts
{
    /// <summary>
    /// Identifies the gameplay system that produced
    /// a SpawnRequest.
    /// </summary>
    public enum SpawnRequestOrigin
    {
        EnemySolver,

        Loot,

        Boss,

        MiniBoss,

        NPC,

        Event,

        Quest,

        ExitPortal,

        Debug,

        Editor,

        Combat
    }
}