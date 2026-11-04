using System;
using System.Collections.Generic;

public sealed class SkillTreeGlobalSkillModifierIntegration : IDisposable
{
    private readonly PlayerSkillLoadout loadout;
    private readonly SkillTreeRuntime skillTreeRuntime;
    private readonly SkillTreeGlobalSkillModifierSourceBuilder sourceBuilder;
    private bool disposed;

    public SkillTreeGlobalSkillModifierIntegration(
        PlayerSkillLoadout loadout,
        SkillTreeRuntime skillTreeRuntime,
        SkillTreeGlobalSkillModifierSourceBuilder sourceBuilder)
    {
        this.loadout = loadout ?? throw new ArgumentNullException(nameof(loadout));
        this.skillTreeRuntime = skillTreeRuntime ?? throw new ArgumentNullException(nameof(skillTreeRuntime));
        this.sourceBuilder = sourceBuilder ?? throw new ArgumentNullException(nameof(sourceBuilder));

        this.skillTreeRuntime.ProgressionChanged += HandleProgressionChanged;
    }

    public bool Refresh()
    {
        if (disposed)
            return false;

        if (!sourceBuilder.TryBuild(
            skillTreeRuntime.Database,
            skillTreeRuntime.State,
            out List<ModifierEntry> modifiers))
        {
            // Fail closed: do not leave stale bonuses active if the source
            // can no longer be reconstructed from the current database.
            loadout.SetGlobalModifiers(null);
            return false;
        }

#if UNITY_EDITOR
        SkillTreeAnalyticsProvenanceRecorder.RegisterEffects(
            skillTreeRuntime.Database, skillTreeRuntime.State, false);
#endif

        loadout.SetGlobalModifiers(modifiers);
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
        skillTreeRuntime.ProgressionChanged -= HandleProgressionChanged;
        loadout.SetGlobalModifiers(null);
    }
}
