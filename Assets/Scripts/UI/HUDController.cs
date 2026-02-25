using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image healthFill;
    [SerializeField] private TMP_Text timerText;

    private PlayerHealth playerHealth;
    private ArenaSpawnDirector arena;
    
    void Update()
    {
        if (arena != null && timerText != null)
        {
            float time = arena.CurrentTime;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void ShowHUD()
    {
        gameObject.SetActive(true);
    }

    public void HideHUD()
    {
        gameObject.SetActive(false);
    }

    public void BindPlayer(PlayerHealth health)
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealth;

        playerHealth = health;

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealth;
            UpdateHealth(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }
    }

    public void BindArena(ArenaSpawnDirector arenaDirector)
    {
        arena = arenaDirector;
    }

    void UpdateHealth(float current, float max)
    {
        if (healthFill != null)
            healthFill.fillAmount = current / max;
    }
}