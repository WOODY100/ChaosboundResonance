using Chaosbound.Content.Currency;
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

    [Header("Currency")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private CurrencyDefinition goldCurrency;

    private PlayerHealth playerHealth;
    private RunManager runManager;
    private int lastSecond = -1;
    private int lastGoldAmount = -1;

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
        UpdateGold(runtimeState);
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

    private void UpdateGold(
        ExpeditionRuntimeState runtimeState)
    {
        if (goldText == null ||
            goldCurrency == null ||
            runtimeState == null)
        {
            return;
        }

        int currentAmount =
            runtimeState.Currency.GetAmount(
                goldCurrency.ContentId);

        if (currentAmount == lastGoldAmount)
            return;

        lastGoldAmount = currentAmount;

        goldText.text =
            currentAmount.ToString("N0");
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

        lastSecond = -1;
        lastGoldAmount = -1;
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