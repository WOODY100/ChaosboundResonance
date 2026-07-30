namespace Chaosbound.Content.Expeditions.Enums.Spawn
{
    /// <summary>
    /// Describes when spawned entities become active.
    /// </summary>
    public enum SpawnActivationPolicy
    {
        Immediate = 0,

        OnCombatStart = 1,

        OnPlayerEnter = 2,

        AfterEvent = 3,

        AfterBossDeath = 4
    }
}