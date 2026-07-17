using Chaosbound.Core.Runtime.Core;
using Chaosbound.Runtime.Population;
using Chaosbound.Runtime.Run;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance;

    [SerializeField] private GameObject gameOverPanel;

    private PlayerHealth player;

    private RuntimeState _runtimeState;
    private IPopulationDirector _populationDirector;
    private RuntimeRunConfig _currentRunConfig;

    private float nextEvaluationTime;

    public RuntimeRunConfig CurrentRunConfig => _currentRunConfig;

    void Awake()
    {
        Instance = this;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (_runtimeState == null ||
            _populationDirector == null)
        {
            return;
        }

        _runtimeState.Advance(Time.deltaTime);

        if (_runtimeState.ElapsedTime < nextEvaluationTime)
        {
            return;
        }

        nextEvaluationTime += 1f;

        PopulationContext context =
            new PopulationContext(_runtimeState.ElapsedTime);

        PopulationIntent intent =
            _populationDirector.Evaluate(context);

        if (intent != null)
        {
            Debug.Log(
                $"[{_runtimeState.ElapsedTime:F1}s] Population Intent: {intent.Formation}");
        }
    }

    public void StartRun(RuntimeRunConfig config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        _currentRunConfig = config;

        _runtimeState = new RuntimeState();

        _populationDirector =
            new PopulationDirector(config.Population);

        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // TODO:
        // Include expedition identity when RuntimeRunConfig exposes it.
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
        _populationDirector = null;

        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        SceneManager.LoadScene("Arena");
    }
}