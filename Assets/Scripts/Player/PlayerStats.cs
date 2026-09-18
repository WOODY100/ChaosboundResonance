using System;
using UnityEngine;

[RequireComponent(typeof(PlayerModifierSystem))]
public class PlayerStats : MonoBehaviour
{
    private PlayerModifierSystem modifierSystem;

    /// <summary>
    /// Raised whenever any player stat is recalculated.
    /// </summary>
    public event Action OnStatsRecalculated;

    private void Awake()
    {
        modifierSystem = GetComponent<PlayerModifierSystem>();
        modifierSystem.OnStatChanged += HandleStatChanged;
    }

    private void OnDestroy()
    {
        if (modifierSystem != null)
            modifierSystem.OnStatChanged -= HandleStatChanged;
    }

    private void HandleStatChanged(StatType statType, float value)
    {
        OnStatsRecalculated?.Invoke();
    }
}