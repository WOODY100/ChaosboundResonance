namespace Chaosbound.Content.Expeditions.Enums.Spawn
{
    /// <summary>
    /// Describes the general placement policy for entities spawned
    /// during an expedition.
    /// </summary>
    public enum SpawnPlacementPolicy
    {
        AroundPlayer = 0,

        AroundCompletionOrigin = 5,

        AroundOrigin = 6,

        AtOrigin = 7
    }
}