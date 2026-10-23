using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace Chaosbound.UI.Loading
{
    public sealed class LoadingScreenController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Slider progressSlider;

        [Header("Timing")]
        [SerializeField, Min(0f)]
        private float minimumLoadingTime = 2.5f;

        [SerializeField, Min(0f)]
        private float fadeDuration = 0.25f;

        [Header("Audio")]
        [SerializeField] private AudioMixerGroup musicMixerGroup;

        private Coroutine progressCoroutine;
        private bool isVisible;

        public float MinimumLoadingTime => minimumLoadingTime;
        public float FadeDuration => fadeDuration;
        public bool IsVisible => isVisible;

        private void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                Debug.LogError(
                    "[LoadingScreenController] CanvasGroup is missing.",
                    this);

                enabled = false;
                return;
            }

            if (progressSlider == null)
            {
                Debug.LogError(
                    "[LoadingScreenController] Progress Slider is missing.",
                    this);

                enabled = false;
                return;
            }

            progressSlider.minValue = 0f;
            progressSlider.maxValue = 1f;
            progressSlider.wholeNumbers = false;
            progressSlider.interactable = false;

            SetProgress(0f);
            SetVisibility(false);
            canvasGroup.alpha = 0f;
        }

        /// <summary>
        /// Immediately shows the loading screen.
        /// Retained for compatibility with existing callers.
        /// </summary>
        public void Show()
        {
            if (!enabled)
                return;

            StopSceneMusic();

            StopProgressAnimation();

            SetProgress(0f);
            SetVisibility(true);
            canvasGroup.alpha = 1f;

            progressCoroutine = StartCoroutine(AnimateProgress());
        }

        /// <summary>
        /// Immediately hides the loading screen.
        /// Retained for compatibility with existing callers.
        /// </summary>
        public void Hide()
        {
            if (!enabled)
                return;

            StopProgressAnimation();

            SetProgress(1f);
            canvasGroup.alpha = 0f;
            SetVisibility(false);
        }

        /// <summary>
        /// Fades the loading screen in and starts its progress animation.
        /// </summary>
        public IEnumerator FadeIn()
        {
            if (!enabled)
                yield break;

            StopSceneMusic();

            StopProgressAnimation();

            SetProgress(0f);
            SetVisibility(true);

            yield return FadeTo(1f);

            progressCoroutine = StartCoroutine(AnimateProgress());
        }

        /// <summary>
        /// Fades the loading screen out before disabling its raycasts.
        /// </summary>
        public IEnumerator FadeOut()
        {
            if (!enabled)
                yield break;

            yield return FadeTo(0f);

            StopProgressAnimation();
            SetProgress(1f);
            SetVisibility(false);
        }

        private IEnumerator FadeTo(float targetAlpha)
        {
            float startAlpha = canvasGroup.alpha;

            if (fadeDuration <= 0f)
            {
                canvasGroup.alpha = targetAlpha;
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                float progress = Mathf.Clamp01(
                    elapsed / fadeDuration);

                // SmoothStep produces a softer visual transition.
                progress = progress * progress * (3f - 2f * progress);

                canvasGroup.alpha = Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    progress);

                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }

        private IEnumerator AnimateProgress()
        {
            float elapsed = 0f;

            if (minimumLoadingTime <= 0f)
            {
                SetProgress(1f);
                progressCoroutine = null;
                yield break;
            }

            while (elapsed < minimumLoadingTime)
            {
                elapsed += Time.unscaledDeltaTime;

                SetProgress(
                    Mathf.Clamp01(elapsed / minimumLoadingTime));

                yield return null;
            }

            SetProgress(1f);
            progressCoroutine = null;
        }

        private void StopProgressAnimation()
        {
            if (progressCoroutine == null)
                return;

            StopCoroutine(progressCoroutine);
            progressCoroutine = null;
        }

        private void SetProgress(float value)
        {
            if (progressSlider != null)
                progressSlider.value = Mathf.Clamp01(value);
        }

        private void SetVisibility(bool visible)
        {
            isVisible = visible;

            if (canvasGroup == null)
                return;

            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        private void StopSceneMusic()
        {
            if (musicMixerGroup == null)
            {
                Debug.LogWarning(
                    "[LoadingScreenController] Music Mixer Group is not assigned.",
                    this);
                return;
            }

            AudioSource[] audioSources =
                FindObjectsByType<AudioSource>(
                    FindObjectsSortMode.None);

            foreach (AudioSource source in audioSources)
            {
                if (source == null)
                    continue;

                if (source.outputAudioMixerGroup == musicMixerGroup)
                    source.Stop();
            }
        }
    }
}