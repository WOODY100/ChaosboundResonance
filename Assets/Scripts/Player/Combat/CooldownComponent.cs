using UnityEngine;

[System.Serializable]
public class CooldownComponent
{
    [SerializeField] private float baseCooldown = 1f;

    private float timer = 0f;

    public float CooldownMultiplier { get; set; } = 1f;

    public bool IsReady => timer <= 0f;

    public float RemainingTime => timer;

    public float NormalizedTime =>
        baseCooldown <= 0f ? 0f : timer / GetFinalCooldown();

    /// <summary>
    /// Advances the cooldown timer.
    /// </summary>
    public void Tick(float deltaTime)
    {
        if (timer <= 0f)
            return;

        timer = Mathf.Max(0f, timer - deltaTime);
    }

    /// <summary>
    /// Starts the cooldown.
    /// </summary>
    public void Trigger()
    {
        timer = GetFinalCooldown();
    }

    public float GetFinalCooldown()
    {
        return baseCooldown * Mathf.Max(0f, CooldownMultiplier);
    }

    public void SetBaseCooldown(float value)
    {
        baseCooldown = Mathf.Max(0f, value);
    }

    /// <summary>
    /// Instantly finishes the cooldown.
    /// </summary>
    public void ForceReady()
    {
        timer = 0f;
    }
}