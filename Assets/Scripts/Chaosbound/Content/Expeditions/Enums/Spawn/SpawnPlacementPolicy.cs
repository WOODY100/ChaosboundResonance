namespace Chaosbound.Content.Expeditions.Enums.Spawn
{
    /// <summary>
    /// Describes the general placement policy for entities spawned
    /// during an expedition.
    /// </summary>
    public enum SpawnPlacementPolicy
    {
        AroundPlayer = 0,

        SpawnPoints = 1,

        ArenaBorder = 2,

        RandomRegion = 3,

        FixedPosition = 4,

        AroundCompletionOrigin = 5
    }
}