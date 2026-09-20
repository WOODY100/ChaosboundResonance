using System;
using Chaosbound.Gameplay.Equipment;
using Chaosbound.Gameplay.Items.Runtime;

public sealed class EquipmentStatsIntegration : IDisposable
{
    private readonly PlayerModifierSystem playerModifierSystem;
    private readonly EquipmentLoadoutRuntime loadout;
    private readonly EquipmentModifierSourceBuilder sourceBuilder;
    private readonly EquipmentProgressionRuntime progressionRuntime;

    public EquipmentStatsIntegration(
        PlayerModifierSystem playerModifierSystem,
        EquipmentLoadoutRuntime loadout,
        EquipmentModifierSourceBuilder sourceBuilder,
        EquipmentProgressionRuntime progressionRuntime)
    {
        if (playerModifierSystem == null)
            throw new ArgumentNullException(
                nameof(playerModifierSystem));

        if (loadout == null)
            throw new ArgumentNullException(
                nameof(loadout));

        if (sourceBuilder == null)
            throw new ArgumentNullException(
                nameof(sourceBuilder));

        if (progressionRuntime == null)
            throw new ArgumentNullException(
                nameof(progressionRuntime));

        this.playerModifierSystem =
            playerModifierSystem;

        this.loadout =
            loadout;

        this.sourceBuilder =
            sourceBuilder;

        this.progressionRuntime =
            progressionRuntime;

        this.loadout.EquipmentChanged +=
            HandleEquipmentChanged;

        this.progressionRuntime.ProgressionChanged +=
            HandleProgressionChanged;
    }

    public bool Refresh()
    {
        if (!sourceBuilder.TryBuild(
                loadout,
                out ModifierSource source))
        {
            playerModifierSystem.RemoveSource(
                ModifierLayer.Meta,
                EquipmentModifierSourceBuilder.SourceId);

            return false;
        }

        playerModifierSystem.AddSource(
            ModifierLayer.Meta,
            source);

        return true;
    }

    private void HandleEquipmentChanged()
    {
        Refresh();
    }

    private void HandleProgressionChanged(
        ItemInstance changedItem)
    {
        if (changedItem == null)
            return;

        if (!loadout.IsEquipped(changedItem))
            return;

        Refresh();
    }

    public void Dispose()
    {
        loadout.EquipmentChanged -=
            HandleEquipmentChanged;

        progressionRuntime.ProgressionChanged -=
            HandleProgressionChanged;
    }
}