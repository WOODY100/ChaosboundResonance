using UnityEngine;
using UnityEngine.UI;

public sealed class HealthOrbController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image orbFill;

    private PlayerHealth playerHealth;

    public void BindPlayer(PlayerHealth health)
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealth;

        playerHealth = health;

        if (playerHealth == null)
            return;

        playerHealth.OnHealthChanged += UpdateHealth;

        UpdateHealth(
            playerHealth.CurrentHealth,
            playerHealth.MaxHealth);
    }

    private void UpdateHealth(float current, float max)
    {
        if (orbFill == null)
            return;

        if (max <= 0f)
        {
            orbFill.fillAmount = 0f;
            return;
        }

        orbFill.fillAmount =
            Mathf.Clamp01(current / max);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealth;
    }
}