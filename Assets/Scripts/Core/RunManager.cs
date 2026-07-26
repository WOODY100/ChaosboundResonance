using Chaosbound.Core.Runtime.State;
using Chaosbound.Content.Expeditions.Runtime.Configs;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance;

    [SerializeField] private GameObject gameOverPanel;

    private PlayerHealth player;

    private RuntimeState _runtimeState;
    private RuntimeExpeditionConfig _currentRunConfig;

    public RuntimeExpeditionConfig CurrentRunConfig => _currentRunConfig;

    void Awake()
    {
        Instance = this;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (_runtimeState == null)
        {
            return;
        }

        _runtimeState.Advance(Time.deltaTime);

        // TODO:
        // Evaluate the Expedition Director once the
        // Director runtime architecture is implemented.
    }

    public void StartRun(RuntimeExpeditionConfig config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        _currentRunConfig = config;

        _runtimeState = new RuntimeState();

        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Debug.Log(
            $"Run started. Difficulty: {config.General.BaseDifficulty}");
    }

    public void BindPlayer(PlayerHealth health)
    {
        if (player != null)
            player.OnDeath -= HandlePlayerDeath;

        player = health;

        if (player != null)
            player.OnDeath += HandlePlayerDeath;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void HandlePlayerDeath()
    {
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void RestartRun()
    {
        _runtimeState = null;

        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        SceneManager.LoadScene("Arena");
    }
}