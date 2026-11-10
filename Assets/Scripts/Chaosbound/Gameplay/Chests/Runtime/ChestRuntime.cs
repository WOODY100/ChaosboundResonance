using System.Collections;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using UnityEngine;
using UnityEngine.Audio;

[DisallowMultipleComponent]
public sealed class ChestRuntime :
    MonoBehaviour,
    IInteractable
{
    [Header("Configuration")]

    [SerializeField]
    private ChestDefinition chestDefinition;

    [Header("References")]

    [SerializeField]
    private Transform cover;

    [SerializeField]
    private Transform lootOrigin;

    [Header("Opening")]

    [SerializeField]
    private float openAngle = 90f;

    [SerializeField]
    private float openDuration = 0.5f;

    [Header("Chest Audio")]

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip openClip;

    [Header("Audio Mixer")]

    [SerializeField]
    private AudioMixerGroup effectsMixerGroup;

    private Quaternion closedRotation;
    private Quaternion openedRotation;

    private bool isOpening;
    private bool isOpened;

    public bool IsOpening => isOpening;
    public bool IsOpened => isOpened;

    private void Awake()
    {
        if (cover != null)
        {
            closedRotation = cover.localRotation;

            openedRotation =
                closedRotation *
                Quaternion.Euler(openAngle, 0f, 0f);
        }

        ConfigureAudio();
    }

    private void ConfigureAudio()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError(
                "[Chest] AudioSource is missing.",
                this);

            return;
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;

        if (effectsMixerGroup != null)
            audioSource.outputAudioMixerGroup = effectsMixerGroup;
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (isOpening || isOpened)
            return;

        if (!ValidateConfiguration())
            return;

        RunManager runManager =
            RunManager.Instance;

        if (runManager == null)
        {
            Debug.LogError(
                "[Chest] RunManager is unavailable.",
                this);

            return;
        }

        ExpeditionRuntimeState runtimeState =
            runManager.ExpeditionRuntimeState;

        if (runtimeState == null)
        {
            Debug.LogWarning(
                "[Chest] No active expedition.",
                this);

            return;
        }

        PendingLoot pendingLoot =
            new PendingLoot(
                chestDefinition.LootDefinition,
                lootOrigin.position);

        StartCoroutine(
            OpenRoutine(runtimeState, pendingLoot));
    }

    private IEnumerator OpenRoutine(
        ExpeditionRuntimeState runtimeState,
        PendingLoot pendingLoot)
    {
        isOpening = true;

        PlayOpenSound();

        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(elapsed / openDuration);

            float smoothProgress =
                progress * progress *
                (3f - 2f * progress);

            cover.localRotation =
                Quaternion.Slerp(
                    closedRotation,
                    openedRotation,
                    smoothProgress);

            yield return null;
        }

        cover.localRotation = openedRotation;

        runtimeState.Reward.AddPendingLoot(
            pendingLoot);

        isOpening = false;
        isOpened = true;
    }

    private void PlayOpenSound()
    {
        if (audioSource == null || openClip == null)
            return;

        audioSource.PlayOneShot(openClip);
    }

    private bool ValidateConfiguration()
    {
        if (chestDefinition == null ||
            chestDefinition.LootDefinition == null)
        {
            Debug.LogError(
                "[Chest] ChestDefinition or LootDefinition is missing.",
                this);

            return false;
        }

        if (cover == null || lootOrigin == null)
        {
            Debug.LogError(
                "[Chest] Cover or LootOrigin reference is missing.",
                this);

            return false;
        }

        return true;
    }
}
