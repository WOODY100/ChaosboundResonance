using UnityEngine;
using System;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    /// <summary>
    /// Gets whether gameplay systems are allowed
    /// to advance the simulation.
    /// </summary>
    public bool CanSimulate
    {
        get
        {
            switch (CurrentState)
            {
                case GameState.Playing:
                    return true;

                default:
                    return false;
            }
        }
    }

    public event Action<GameState> OnStateChanged;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SetState(GameState.Playing);
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;

        ApplyState(newState);

        OnStateChanged?.Invoke(newState);
    }

    private void ApplyState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;

            case GameState.LevelUp:
                Time.timeScale = 0f;
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }
}