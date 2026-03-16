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
    private int lastSecond = -1;

    void Update()
    {
        if (arena == null || timerText == null)
            return;

        float time = arena.CurrentTime;
        int totalSeconds = Mathf.FloorToInt(time);

        if (totalSeconds == lastSecond)
            return;

        lastSecond = totalSeconds;

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
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