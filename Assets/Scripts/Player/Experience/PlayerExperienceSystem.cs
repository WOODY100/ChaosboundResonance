using UnityEngine;
using System;

public class PlayerExperienceSystem : MonoBehaviour
{
    [Header("XP Curve")]
    [SerializeField] private float baseXP = 10f;
    [SerializeField] private float growthFactor = 0.35f;

    public int CurrentLevel { get; private set; } = 1;
    public float CurrentXP { get; private set; }
    public float RequiredXP { get; private set; }

    public float NormalizedXP =>
        RequiredXP <= 0f ? 0f : CurrentXP / RequiredXP;

    public event Action<float, float> OnXPChanged;
    public event Action<int> OnLevelUp;

    void Awake()
    {
        RecalculateRequiredXP();
        OnXPChanged?.Invoke(CurrentXP, RequiredXP);
    }

    // ===============================
    // PUBLIC API
    // ===============================

    public void AddXP(float amount)
    {
        if (amount <= 0f)
            return;

        CurrentXP += amount;

        CheckLevelUp();
    }

    public void ResetProgression()
    {
        CurrentLevel = 1;
        CurrentXP = 0f;

        RecalculateRequiredXP();

        OnXPChanged?.Invoke(CurrentXP, RequiredXP);
    }

    // ===============================
    // INTERNAL LOGIC
    // ===============================

    private void CheckLevelUp()
    {
        while (CurrentXP >= RequiredXP)
        {
            CurrentXP -= RequiredXP;
            CurrentLevel++;

            RecalculateRequiredXP();

            OnLevelUp?.Invoke(CurrentLevel);
        }

        OnXPChanged?.Invoke(CurrentXP, RequiredXP);
    }

    private void OnValidate()
    {
        baseXP = Mathf.Max(1f, baseXP);
        growthFactor = Mathf.Max(0f, growthFactor);
    }

    private void RecalculateRequiredXP()
    {
        RequiredXP = Mathf.Max(
            1f,
            baseXP * (1f + (CurrentLevel - 1) * growthFactor));
    }
}