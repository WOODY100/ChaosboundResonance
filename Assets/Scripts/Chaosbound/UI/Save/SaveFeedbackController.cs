using System.Collections;
using TMPro;
using UnityEngine;
using System;

public sealed class SaveFeedbackController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject feedbackRoot;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text statusText;

    [Header("Timing")]
    [SerializeField, Min(0f)] private float successDisplayTime = 2f;
    [SerializeField, Min(0f)] private float errorDisplayTime = 3f;
    [SerializeField, Min(0f)] private float fadeDuration = 0.25f;
    
    [SerializeField, Min(0f)]
    private float minimumSavingDisplayTime = 0.5f;

    private Coroutine feedbackCoroutine;

    private void Awake()
    {
        if (feedbackRoot != null)
            feedbackRoot.SetActive(false);
    }

    private void OnDisable()
    {
        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
            feedbackCoroutine = null;
        }

        if (feedbackRoot != null)
            feedbackRoot.SetActive(false);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void ShowSaving()
    {
        StopFeedbackCoroutine();

        if (feedbackRoot != null)
            feedbackRoot.SetActive(true);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (statusText != null)
            statusText.text = "Saving...";
    }

    public void ShowResult(bool success)
    {
        StopFeedbackCoroutine();

        if (feedbackRoot != null)
            feedbackRoot.SetActive(true);

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (statusText != null)
            statusText.text = success ? "Game saved" : "Save failed";

        feedbackCoroutine = StartCoroutine(
            DisplayResultRoutine(success));
    }

    private IEnumerator DisplayResultRoutine(bool success)
    {
        float displayTime = success
            ? successDisplayTime
            : errorDisplayTime;

        yield return new WaitForSecondsRealtime(displayTime);

        if (canvasGroup != null && fadeDuration > 0f)
        {
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                canvasGroup.alpha = Mathf.Lerp(
                    startAlpha,
                    0f,
                    Mathf.Clamp01(elapsed / fadeDuration));

                yield return null;
            }

            canvasGroup.alpha = 0f;
        }

        if (feedbackRoot != null)
            feedbackRoot.SetActive(false);

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        feedbackCoroutine = null;
    }

    private void StopFeedbackCoroutine()
    {
        if (feedbackCoroutine == null)
            return;

        StopCoroutine(feedbackCoroutine);
        feedbackCoroutine = null;
    }

    public void RunSaveOperation(Action saveOperation)
    {
        if (saveOperation == null)
        {
            Debug.LogError(
                "[SaveFeedbackController] Save operation is null.",
                this);

            ShowResult(false);
            return;
        }

        StopFeedbackCoroutine();

        ShowSaving();

        feedbackCoroutine = StartCoroutine(
            RunSaveOperationRoutine(saveOperation));
    }

    private IEnumerator RunSaveOperationRoutine(
        Action saveOperation)
    {
        float elapsed = 0f;

        while (elapsed < minimumSavingDisplayTime)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        bool success = false;

        try
        {
            saveOperation.Invoke();
            success = true;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "[SaveFeedbackController] Save operation failed. " +
                exception,
                this);
        }

        ShowResult(success);
    }
}