using System;

public sealed class SkillTreeStatsIntegration : IDisposable
{
    private readonly PlayerModifierSystem playerModifierSystem;
    private readonly SkillTreeRuntime skillTreeRuntime;
    private readonly SkillTreeStatModifierSourceBuilder sourceBuilder;

    private bool disposed;

    public SkillTreeStatsIntegration(
        PlayerModifierSystem playerModifierSystem,
        SkillTreeRuntime skillTreeRuntime,
        SkillTreeStatModifierSourceBuilder sourceBuilder)
    {
        if (playerModifierSystem == null)
            throw new ArgumentNullException(
                nameof(playerModifierSystem));

        if (skillTreeRuntime == null)
            throw new ArgumentNullException(
                nameof(skillTreeRuntime));

        if (sourceBuilder == null)
            throw new ArgumentNullException(
                nameof(sourceBuilder));

        this.playerModifierSystem = playerModifierSystem;
        this.skillTreeRuntime = skillTreeRuntime;
        this.sourceBuilder = sourceBuilder;

        this.skillTreeRuntime.ProgressionChanged +=
            HandleProgressionChanged;
    }

    public bool Refresh()
    {
        if (disposed)
            return false;

        if (!sourceBuilder.TryBuild(
            skillTreeRuntime.Database,
            skillTreeRuntime.State,
            out ModifierSource source))
        {
            playerModifierSystem.RemoveSource(
                ModifierLayer.Meta,
                SkillTreeStatModifierSourceBuilder.SourceId);

            return false;
        }

#if UNITY_EDITOR
        SkillTreeAnalyticsProvenanceRecorder.RegisterEffects(
            skillTreeRuntime.Database, skillTreeRuntime.State, true);
#endif

        // AddSource replaces an existing source with the same ID.
        // An empty source is intentional: it clears stale tree bonuses.
        playerModifierSystem.AddSource(
            ModifierLayer.Meta,
            source);

        return true;
    }

    private void HandleProgressionChanged()
    {
        Refresh();
    }

    public void Dispose()
    {
        if (disposed)
            return;

        disposed = true;

        skillTreeRuntime.ProgressionChanged -=
            HandleProgressionChanged;

        playerModifierSystem.RemoveSource(
            ModifierLayer.Meta,
            SkillTreeStatModifierSourceBuilder.SourceId);
    }
}
