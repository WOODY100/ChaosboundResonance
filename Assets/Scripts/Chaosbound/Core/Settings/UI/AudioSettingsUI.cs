using Chaosbound.Core.Audio;
using Chaosbound.Core.Composition;
using Chaosbound.Core.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.Core.Settings.UI
{
    public sealed class AudioSettingsUI : MonoBehaviour
    {
        [Header("Volume Sliders")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider ambienceVolumeSlider;
        [SerializeField] private Slider effectsVolumeSlider;
        [SerializeField] private Slider uiVolumeSlider;

        [Header("Mute")]
        [SerializeField] private Toggle muteAllToggle;

        private GameSettingsRuntime settingsRuntime;
        private AudioManager audioManager;

        private void OnEnable()
        {
            ResolveDependencies();

            if (settingsRuntime != null)
                settingsRuntime.SettingsChanged += HandleSettingsChanged;

            RefreshFromSettings();
        }

        private void OnDisable()
        {
            if (settingsRuntime != null)
                settingsRuntime.SettingsChanged -= HandleSettingsChanged;
        }

        public void RefreshFromSettings()
        {
            if (!EnsureSettingsRuntime())
                return;

            GameSettings settings = settingsRuntime.Settings;

            SetSliderWithoutNotify(
                masterVolumeSlider, settings.MasterVolume);

            SetSliderWithoutNotify(
                musicVolumeSlider, settings.MusicVolume);

            SetSliderWithoutNotify( 
                ambienceVolumeSlider, settings.AmbienceVolume);

            SetSliderWithoutNotify(
                effectsVolumeSlider, settings.EffectsVolume);

            SetSliderWithoutNotify(
                uiVolumeSlider, settings.UIVolume);

            if (muteAllToggle != null)
            {
                muteAllToggle.SetIsOnWithoutNotify(
                    settings.MuteAll);
            }
        }

        public void Apply()
        {
            if (!EnsureSettingsRuntime())
                return;

            SaveFeedbackController feedbackController =
                ResolveSaveFeedbackController();

            if (feedbackController == null)
            {
                Debug.LogError(
                    "[AudioSettingsUI] SaveFeedbackController was not found.",
                    this);
                return;
            }

            GameSettings current = settingsRuntime.Settings;

            GameSettings newSettings = new GameSettings
            {
                ConfirmWorldItemDrop =
                    current.ConfirmWorldItemDrop,

                ShowDamageNumbers =
                    current.ShowDamageNumbers,

                ShowEnemyHealthBars =
                    current.ShowEnemyHealthBars,

                Vibration =
                    current.Vibration,

                MasterVolume = GetSliderValue(
                    masterVolumeSlider, current.MasterVolume),

                MusicVolume = GetSliderValue(
                    musicVolumeSlider, current.MusicVolume),

                AmbienceVolume = GetSliderValue(
                    ambienceVolumeSlider, current.AmbienceVolume),

                EffectsVolume = GetSliderValue(
                    effectsVolumeSlider, current.EffectsVolume),

                UIVolume = GetSliderValue(
                    uiVolumeSlider, current.UIVolume),

                MuteAll = muteAllToggle != null
                    ? muteAllToggle.isOn
                    : current.MuteAll
            };

            feedbackController.RunSaveOperation(
                () => settingsRuntime.ApplySettings(newSettings));
        }

        public void RestoreDefaults()
        {
            if (!EnsureSettingsRuntime())
                return;

            SaveFeedbackController feedbackController =
                ResolveSaveFeedbackController();

            if (feedbackController == null)
            {
                Debug.LogError(
                    "[AudioSettingsUI] SaveFeedbackController was not found.",
                    this);
                return;
            }

            feedbackController.RunSaveOperation(
                () =>
                {
                    settingsRuntime.ApplySettings(
                        CreateDefaultsPreservingGameplay(
                            settingsRuntime.Settings));

                    RefreshFromSettings();
                });
        }

        private GameSettings CreateDefaultsPreservingGameplay(
            GameSettings current)
        {
            return new GameSettings
            {
                ConfirmWorldItemDrop =
                    current.ConfirmWorldItemDrop,

                ShowDamageNumbers =
                    current.ShowDamageNumbers,

                ShowEnemyHealthBars =
                    current.ShowEnemyHealthBars,

                Vibration =
                    current.Vibration,

                MasterVolume = 0.8f,
                MusicVolume = 0.8f,
                AmbienceVolume = 0.8f,
                EffectsVolume = 0.8f,
                UIVolume = 0.8f,
                MuteAll = false
            };
        }

        private void HandleSettingsChanged(GameSettings settings)
        {
            RefreshFromSettings();
        }

        private void ResolveDependencies()
        {
            BootstrapContext context = BootstrapContext.Current;

            settingsRuntime = context != null
                ? context.GameSettingsRuntime
                : null;

            audioManager = context != null
                ? context.AudioManager
                : null;
        }

        private bool EnsureSettingsRuntime()
        {
            if (settingsRuntime != null &&
                settingsRuntime.IsInitialized)
            {
                return true;
            }

            ResolveDependencies();

            return settingsRuntime != null &&
                   settingsRuntime.IsInitialized;
        }

        private SaveFeedbackController ResolveSaveFeedbackController()
        {
            BootstrapContext context = BootstrapContext.Current;

            if (context == null)
                return null;

            return context.GetComponentInChildren<
                SaveFeedbackController>(true);
        }

        private float GetSliderValue(
            Slider slider,
            float fallback)
        {
            return slider != null
                ? Mathf.Clamp01(slider.value)
                : fallback;
        }

        private void SetSliderWithoutNotify(
            Slider slider,
            float value)
        {
            if (slider == null)
                return;

            slider.SetValueWithoutNotify(
                Mathf.Clamp01(value));
        }
    }
}