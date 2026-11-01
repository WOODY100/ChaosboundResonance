using Chaosbound.Core.Composition;
using UnityEngine;

public sealed class SkillTreeStatsRuntimeComposition : MonoBehaviour
{
    private SkillTreeStatsIntegration integration;
    private SkillTreeGlobalSkillModifierIntegration globalSkillIntegration;

    private void Start()
    {
        Compose();
    }

    private void OnDestroy()
    {
        DisposeIntegration();
    }

    private void Compose()
    {
        BootstrapContext bootstrapContext =
            BootstrapContext.Current;

        if (bootstrapContext == null)
        {
            Debug.LogError(
                "[Skill Tree Stats Runtime Composition] " +
                "BootstrapContext is not available.",
                this);

            return;
        }

        SkillTreeRuntime skillTreeRuntime =
            bootstrapContext.SkillTreeRuntime;

        if (skillTreeRuntime == null)
        {
            Debug.LogError(
                "[Skill Tree Stats Runtime Composition] " +
                "SkillTreeRuntime is not available.",
                this);

            return;
        }

        PlayerModifierSystem playerModifierSystem =
            GetComponent<PlayerModifierSystem>();

        if (playerModifierSystem == null)
        {
            Debug.LogError(
                "[Skill Tree Stats Runtime Composition] " +
                "PlayerModifierSystem was not found on this GameObject.",
                this);

            return;
        }

        PlayerSkillLoadout skillLoadout =
            GetComponent<PlayerSkillLoadout>();

        if (skillLoadout == null)
        {
            Debug.LogError(
                "[Skill Tree Stats Runtime Composition] " +
                "PlayerSkillLoadout was not found on this GameObject.",
                this);
            return;
        }

        SkillTreeStatModifierSourceBuilder sourceBuilder =
            new SkillTreeStatModifierSourceBuilder();

        integration =
            new SkillTreeStatsIntegration(
                playerModifierSystem,
                skillTreeRuntime,
                sourceBuilder);

        if (!integration.Refresh())
        {
            Debug.LogError(
                "[Skill Tree Stats Runtime Composition] " +
                "Failed to build the Skill Tree stat modifier source.",
                this);
        }

        globalSkillIntegration =
            new SkillTreeGlobalSkillModifierIntegration(
                skillLoadout,
                skillTreeRuntime,
                new SkillTreeGlobalSkillModifierSourceBuilder());

        if (!globalSkillIntegration.Refresh())
        {
            Debug.LogError(
                "[Skill Tree Stats Runtime Composition] " +
                "Failed to build the global Skill Tree modifier set.",
                this);
        }
    }

    private void DisposeIntegration()
    {
        if (integration != null)
        {
            integration.Dispose();
            integration = null;
        }

        if (globalSkillIntegration != null)
        {
            globalSkillIntegration.Dispose();
            globalSkillIntegration = null;
        }
    }
}
