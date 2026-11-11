using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using UnityEngine;

/// <summary>
/// Physical, automatically collected producer of an expedition modifier.
/// It delegates all stacking and lifetime rules to ExpeditionModifierDomainDirector.
/// </summary>
public sealed class PowerUpOrb : AutoPickupBehaviour
{
    [Header("Modifier")]
    [SerializeField]
    private ExpeditionModifierDefinition modifierDefinition;

    [Header("Visual")]
    [SerializeField]
    private Transform visualRoot;

    [SerializeField]
    private SpriteRenderer iconRenderer;

    private readonly ExpeditionModifierDomainDirector director =
        new ExpeditionModifierDomainDirector();

    public ExpeditionModifierDefinition ModifierDefinition =>
        modifierDefinition;

    public bool IsConfigured =>
        modifierDefinition != null &&
        modifierDefinition.TryValidate(out _);

    protected override void OnEnable()
    {
        base.OnEnable();
        ApplyDefinitionVisual();
    }

    protected override float GetPickupRadius()
    {
        return GetPlayerPickupRadius();
    }

    protected override void OnAutoPickupTriggered()
    {
        if (Player == null)
            return;

        RunManager runManager = RunManager.Instance;
        if (runManager == null ||
            runManager.ExpeditionRuntimeState == null)
        {
            ResetPickupTrigger();
            return;
        }

        if (TryApply(runManager.ExpeditionRuntimeState))
            ReturnToPool();
        else
            ResetPickupTrigger();
    }

    /// <summary>
    /// Applies this orb's definition to the supplied expedition.
    /// Kept internal for isolated producer regression tests.
    /// </summary>
    internal bool TryApply(ExpeditionRuntimeState runtimeState)
    {
        if (runtimeState == null)
            throw new System.ArgumentNullException(nameof(runtimeState));

        if (!IsConfigured)
            return false;

        string sourceId =
            $"powerup-orb:{modifierDefinition.ContentId}:{GetInstanceID()}";

        ExpeditionModifierApplyResult result =
            director.ApplyModifier(
                runtimeState.Modifiers,
                modifierDefinition,
                sourceId,
                runtimeState.ElapsedTime);

        if (!result.Succeeded)
        {
            Debug.LogWarning(
                $"PowerUpOrb '{name}' could not apply definition " +
                $"'{modifierDefinition.ContentId}': {result.Error}",
                this);
            return false;
        }

        return true;
    }

    private void ApplyDefinitionVisual()
    {
        if (iconRenderer == null)
            return;

        iconRenderer.sprite =
            modifierDefinition != null
                ? modifierDefinition.Icon
                : null;
    }

    private void LateUpdate()
    {
        if (visualRoot == null)
            return;

        Camera camera = Camera.main;
        if (camera == null)
            return;

        // The visual plane faces the camera; gameplay root and collider
        // remain aligned with the world.
        visualRoot.rotation = Quaternion.LookRotation(
            -camera.transform.forward,
            camera.transform.up);
    }

    protected override void ResetPooledState()
    {
        base.ResetPooledState();
        ApplyDefinitionVisual();
    }

    private void OnValidate()
    {
        if (iconRenderer == null && visualRoot != null)
            iconRenderer = visualRoot.GetComponentInChildren<SpriteRenderer>(true);
    }
}
