using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using UnityEngine;

public sealed class ModifierInteractable :
    MonoBehaviour,
    IInteractable
{
    [Header("Identity")]
    [SerializeField]
    private string contentId;

    [Header("Modifier Definition")]
    [SerializeField]
    private ExpeditionModifierDefinition modifierDefinition;

    private readonly ExpeditionModifierDomainDirector director =
        new ExpeditionModifierDomainDirector();

    public string ContentId => contentId;

    public ExpeditionModifierDefinition ModifierDefinition =>
        modifierDefinition;

    public bool IsConfigured
    {
        get
        {
            if (string.IsNullOrWhiteSpace(contentId) ||
                modifierDefinition == null)
            {
                return false;
            }

            return modifierDefinition.TryValidate(out _);
        }
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!IsConfigured)
        {
            Debug.LogError(
                $"ModifierInteractable '{name}' is not configured or its definition is invalid.",
                this);
            return;
        }

        RunManager runManager = RunManager.Instance;
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
                $"ModifierInteractable '{name}' cannot be used because there is no active expedition.",
                this);
            return;
        }

        TryApply(runtimeState);
    }

    /// <summary>
    /// Applies this producer's definition to an expedition.
    /// Exposed internally so the producer contract can be regression-tested
    /// without requiring a live RunManager or player interaction.
    /// </summary>
    internal bool TryApply(ExpeditionRuntimeState runtimeState)
    {
        if (runtimeState == null)
            throw new ArgumentNullException(nameof(runtimeState));

        if (!IsConfigured)
            return false;

        if (runtimeState.InteractableUsage.HasBeenUsed(contentId))
            return false;

        ExpeditionModifierApplyResult result =
            director.ApplyModifier(
                runtimeState.Modifiers,
                modifierDefinition,
                contentId,
                runtimeState.ElapsedTime);

        if (!result.Succeeded)
        {
            Debug.LogWarning(
                $"ModifierInteractable '{name}' could not apply definition " +
                $"'{modifierDefinition.ContentId}': {result.Error}",
                this);
            return false;
        }

        runtimeState.InteractableUsage.MarkUsed(contentId);
        return true;
    }

    private void OnValidate()
    {
        contentId = contentId == null
            ? string.Empty
            : contentId.Trim();
    }
}
