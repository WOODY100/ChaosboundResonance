using Chaosbound.Gameplay.ExpeditionRuntime.Director;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using System;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private HealthOrbController healthOrb;

    private PlayerHealth playerHealth;
    private RunManager runManager;
    private int lastSecond = -1;

    private void Update()
    {
        if (runManager == null)
            return;

        ExpeditionDirector director =
            runManager.ExpeditionDirector;

        if (director == null)
            return;

        if (!director.IsRunning)
            return;

        ExpeditionRuntimeState runtimeState =
            director.RuntimeState;

        if (runtimeState == null)
            return;

        UpdateTimer(runtimeState.ElapsedTime);
    }

    private void UpdateTimer(
        TimeSpan elapsedTime)
    {
        if (timerText == null)
            return;

        int totalSeconds =
            (int)elapsedTime.TotalSeconds;

        if (totalSeconds == lastSecond)
            return;

        lastSecond = totalSeconds;

        timerText.text =
            elapsedTime.ToString(@"mm\:ss");
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
        playerHealth = health;

        if (playerHealth != null &&
            healthOrb != null)
        {
            healthOrb.BindPlayer(playerHealth);
        }
    }

    public void BindRunManager(
        RunManager manager)
    {
        runManager = manager;
    }

    public void Initialize(
        PlayerHealth player,
        RunManager manager)
    {
        ShowHUD();

        BindPlayer(player);
        BindRunManager(manager);
    }

    public void Shutdown()
    {
        HideHUD();
    }
}