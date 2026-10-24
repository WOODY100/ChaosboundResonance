using System;
using UnityEngine;
using UnityEngine.Audio;
using Chaosbound.Core.Settings;

namespace Chaosbound.Core.Audio
{
    public sealed class AudioManager : MonoBehaviour
    {
        private const float MinimumDecibels = -80f;
        private const float MinimumLinearVolume = 0.0001f;

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Exposed Parameters")]
        [SerializeField]
        private string masterVolumeParameter =
            "MasterVolume";

        [SerializeField]
        private string musicVolumeParameter =
            "MusicVolume";

        [SerializeField]
        private string ambienceVolumeParameter =
            "AmbienceVolume";

        [SerializeField]
        private string effectsVolumeParameter =
            "EffectsVolume";

        [SerializeField]
        private string uiVolumeParameter =
            "UIVolume";

        private GameSettingsRuntime settingsRuntime;
        private bool isInitialized;

        public bool IsInitialized => isInitialized;

        public void Initialize(GameSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (audioMixer == null)
            {
                Debug.LogError(
                    "[AudioManager] AudioMixer is not assigned.",
                    this);
                return;
            }

            ApplySettings(settings);
            isInitialized = true;
        }

        public void Bind(GameSettingsRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));

            if (settingsRuntime == runtime)
            {
                if (runtime.IsInitialized)
                    ApplySettings(runtime.Settings);

                return;
            }

            Unbind();

            settingsRuntime = runtime;
            settingsRuntime.SettingsChanged += HandleSettingsChanged;

            if (settingsRuntime.IsInitialized)
            {
                ApplySettings(settingsRuntime.Settings);
            }
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void Unbind()
        {
            if (settingsRuntime == null)
                return;

            settingsRuntime.SettingsChanged -= HandleSettingsChanged;
            settingsRuntime = null;
        }

        private void HandleSettingsChanged(GameSettings settings)
        {
            ApplySettings(settings);
        }

        public void ApplySettings(GameSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (audioMixer == null)
            {
                Debug.LogError(
                    "[AudioManager] AudioMixer is not assigned.",
                    this);
                return;
            }

            float master = settings.MuteAll ? 0f : settings.MasterVolume;
            float music = settings.MuteAll ? 0f : settings.MusicVolume;
            float ambience = settings.MuteAll ? 0f : settings.AmbienceVolume;
            float effects = settings.MuteAll ? 0f : settings.EffectsVolume;
            float ui = settings.MuteAll ? 0f : settings.UIVolume;

            SetVolume(masterVolumeParameter, master);
            SetVolume(musicVolumeParameter, music);
            SetVolume(ambienceVolumeParameter, ambience);
            SetVolume(effectsVolumeParameter, effects);
            SetVolume(uiVolumeParameter, ui);
        }

        private void SetVolume(
            string parameterName,
            float linearVolume)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                Debug.LogError(
                    "[AudioManager] An exposed volume parameter has an empty name.",
                    this);
                return;
            }

            float clampedVolume = Mathf.Clamp01(linearVolume);

            float decibels =
                clampedVolume <= MinimumLinearVolume
                    ? MinimumDecibels
                    : Mathf.Max(
                        MinimumDecibels,
                        20f * Mathf.Log10(clampedVolume));

            if (!audioMixer.SetFloat(parameterName, decibels))
            {
                Debug.LogError(
                    $"[AudioManager] Could not set Audio Mixer parameter " +
                    $"'{parameterName}'. Check its exposed name.",
                    this);
            }
        }
    }
}