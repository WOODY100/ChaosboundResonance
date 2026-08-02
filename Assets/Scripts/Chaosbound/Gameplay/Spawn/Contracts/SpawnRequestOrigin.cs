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

        NPC,

        Event,

        Quest,

        Debug,

        Editor
    }
}