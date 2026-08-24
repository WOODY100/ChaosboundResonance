using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ModifierInteractable :
    MonoBehaviour,
    IInteractable
{
    [Header("Identity")]

    [SerializeField]
    private string contentId;

    [Header("Modifier")]

    [SerializeField]
    private List<ModifierEffectConfiguration> effects =
        new List<ModifierEffectConfiguration>();

    [SerializeField]
    private ExpeditionModifierLifetime lifetime =
        ExpeditionModifierLifetime.Expedition;

    [SerializeField]
    private float durationSeconds;

    public string ContentId =>
        contentId;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(contentId) &&
        effects != null &&
        effects.Count > 0;

    public void Interact(
        PlayerInteractor interactor)
    {
        if (!IsConfigured)
        {
            Debug.LogError(
                $"ModifierInteractable '{name}' is not configured.",
                this);

            return;
        }

        RunManager runManager =
            RunManager.Instance;

        if (runManager == null)
        {
            Debug.LogError(
                $"ModifierInteractable '{name}' could not find RunManager.",
                this);

            return;
        }

        ExpeditionRuntimeState runtimeState =
            runManager.ExpeditionRuntimeState;

        if (runtimeState == null)
        {
            Debug.LogWarning(
                $"ModifierInteractable '{name}' cannot be used because " +
                "there is no active expedition.",
                this);

            return;
        }

        if (runtimeState.InteractableUsage.HasBeenUsed(
            contentId))
        {
            return;
        }

        ExpeditionModifier modifier =
            BuildModifier(
                runtimeState.ElapsedTime);

        ExpeditionModifierDomainDirector director =
            new ExpeditionModifierDomainDirector();

        director.AddModifier(
            runtimeState.Modifiers,
            modifier);

        runtimeState.InteractableUsage.MarkUsed(
            contentId);
    }

    private ExpeditionModifier BuildModifier(
        TimeSpan createdAt)
    {
        List<ExpeditionModifierEffect> modifierEffects =
            new List<ExpeditionModifierEffect>();

        foreach (
            ModifierEffectConfiguration configuration
            in effects)
        {
            if (configuration == null)
                continue;

            modifierEffects.Add(
                new ExpeditionModifierEffect(
                    configuration.Target,
                    configuration.StatId,
                    configuration.Percent));
        }

        if (modifierEffects.Count == 0)
        {
            throw new InvalidOperationException(
                $"ModifierInteractable '{name}' does not contain " +
                "any valid modifier effects.");
        }

        TimeSpan duration =
            lifetime ==
            ExpeditionModifierLifetime.Timed
                ? TimeSpan.FromSeconds(
                    Mathf.Max(
                        0f,
                        durationSeconds))
                : TimeSpan.Zero;

        return new ExpeditionModifier(
            modifierEffects,
            lifetime,
            createdAt,
            duration);
    }

    private void OnValidate()
    {
        if (effects == null)
        {
            effects =
                new List<ModifierEffectConfiguration>();
        }

        durationSeconds =
            Mathf.Max(
                0f,
                durationSeconds);
    }
}