using System;
using System.Collections.Generic;
using Chaosbound.Core.Composition;
using Chaosbound.Gameplay.Save;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Chaosbound.Core.Settings.UI
{
    public sealed class VideoSettingsUI : MonoBehaviour
    {
        [Header("Video Controls")]
        [SerializeField] private TMP_Dropdown windowModeDropdown;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle vSyncToggle;
        [SerializeField] private TMP_Dropdown frameRateDropdown;
        [SerializeField] private TMP_Dropdown qualityDropdown;

        [Header("Presets")]
        [SerializeField] private TMP_Dropdown presetDropdown;

        private VideoSettingsRuntime runtime;
        private bool isRefreshing;

        private VideoSettingsConfirmationController confirmationController;

        private static readonly int[] FrameRates =
        {
            30, 60, 120, 144, 0
        };

        private static readonly Vector2Int[] Resolutions =
        {
            new Vector2Int(1920, 1080),
            new Vector2Int(1280, 720),
            new Vector2Int(2560, 1440),
            new Vector2Int(3840, 2160)
        };

        private void OnEnable()
        {
            ResolveRuntime();

            if (runtime == null || !runtime.IsInitialized)
            {
                Debug.LogWarning(
                    "[VideoSettingsUI] VideoSettingsRuntime " +
                    "is not available.",
                    this);
                return;
            }

            ConfigureDropdowns();
            RegisterListeners();
            RefreshFromSettings();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void ResolveRuntime()
        {
            BootstrapContext context = BootstrapContext.Current;

            runtime = context != null
                ? context.VideoSettingsRuntime
                : null;

            confirmationController = context != null
                ? context.GetComponentInChildren
                    <VideoSettingsConfirmationController>(true)
                : null;
        }

        private void ConfigureDropdowns()
        {
            SetOptions(
                windowModeDropdown,
                new[] {
                    "Pantalla Completa",
                    "Ventana Sin Bordes",
                    "Ventana"
                });

            SetOptions(
                resolutionDropdown,
                new[] {
                    "(Full HD) 1920 X 1080",
                    "(720p) 1280 X 720",
                    "(2K) 2560 X 1440",
                    "(4K) 3840 X 2160"
                });

            SetOptions(
                frameRateDropdown,
                new[] {
                    "30", "60", "120", "144", "Ilimitado"
                });

            if (qualityDropdown != null)
            {
                List<string> qualityOptions =
                    new List<string>();

                qualityOptions.Add("Baja");
                qualityOptions.Add("Media");
                qualityOptions.Add("Alta");
                qualityOptions.Add("Ultra");

                qualityDropdown.ClearOptions();
                qualityDropdown.AddOptions(qualityOptions);
            }

            SetOptions(
                presetDropdown,
                new[] {
                    "Bajo",
                    "Equilibrado",
                    "Alto",
                    "Ultra",
                    "Personalizado"
                });
        }

        private void RegisterListeners()
        {
            if (windowModeDropdown != null)
                windowModeDropdown.onValueChanged.AddListener(OnControlChanged);

            if (resolutionDropdown != null)
                resolutionDropdown.onValueChanged.AddListener(OnControlChanged);

            if (vSyncToggle != null)
                vSyncToggle.onValueChanged.AddListener(OnToggleChanged);

            if (frameRateDropdown != null)
                frameRateDropdown.onValueChanged.AddListener(OnControlChanged);

            if (qualityDropdown != null)
                qualityDropdown.onValueChanged.AddListener(OnControlChanged);

            if (presetDropdown != null)
                presetDropdown.onValueChanged.AddListener(OnPresetChanged);
        }

        private void RemoveListeners()
        {
            if (windowModeDropdown != null)
                windowModeDropdown.onValueChanged.RemoveListener(OnControlChanged);

            if (resolutionDropdown != null)
                resolutionDropdown.onValueChanged.RemoveListener(OnControlChanged);

            if (vSyncToggle != null)
                vSyncToggle.onValueChanged.RemoveListener(OnToggleChanged);

            if (frameRateDropdown != null)
                frameRateDropdown.onValueChanged.RemoveListener(OnControlChanged);

            if (qualityDropdown != null)
                qualityDropdown.onValueChanged.RemoveListener(OnControlChanged);

            if (presetDropdown != null)
                presetDropdown.onValueChanged.RemoveListener(OnPresetChanged);
        }

        public void RefreshFromSettings()
        {
            if (runtime == null || runtime.Settings == null)
                return;

            isRefreshing = true;

            VideoSettingsData data = runtime.Settings;

            SetDropdownWithoutNotify(
                windowModeDropdown,
                (int)data.WindowMode);

            SetDropdownWithoutNotify(
                resolutionDropdown,
                FindResolutionIndex(
                    data.ResolutionWidth,
                    data.ResolutionHeight));

            SetToggleWithoutNotify(vSyncToggle, data.VSync);

            SetDropdownWithoutNotify(
                frameRateDropdown,
                FindFrameRateIndex(data.TargetFrameRate));

            SetDropdownWithoutNotify(
                qualityDropdown,
                QualityLevelToDropdownIndex(data.QualityLevel));

            SetDropdownWithoutNotify(
                presetDropdown,
                (int)data.Preset);

            isRefreshing = false;
        }

        public void Apply()
        {
            if (!EnsureRuntime())
                return;

            if (confirmationController == null)
            {
                ResolveRuntime();
            }

            if (confirmationController == null)
            {
                Debug.LogError(
                    "[VideoSettingsUI] VideoSettingsConfirmationController " +
                    "was not found.",
                    this);
                return;
            }

            VideoSettingsData data = ReadControls();
            data.Preset = DeterminePreset(data);

            if (!runtime.TryApplySettings(data))
                return;

            bool started = confirmationController.BeginPreview(
                runtime,
                () =>
                {
                    SaveFeedbackController feedbackController =
                        ResolveSaveFeedbackController();

                    if (feedbackController != null)
                    {
                        feedbackController.RunSaveOperation(
                            RefreshFromSettings);
                    }
                    else
                    {
                        RefreshFromSettings();
                    }
                },
                RefreshFromSettings);

            if (!started)
            {
                runtime.RevertPreviewSettings();
                RefreshFromSettings();
            }
        }

        public void RestoreDefaults()
        {
            if (!EnsureRuntime())
                return;

            SaveFeedbackController feedbackController =
                ResolveSaveFeedbackController();

            if (feedbackController == null)
            {
                Debug.LogError(
                    "[VideoSettingsUI] SaveFeedbackController was not found.",
                    this);
                return;
            }

            VideoSettingsData defaults = new VideoSettingsData
            {
                Preset = VideoPreset.Balanced,
                WindowMode = VideoWindowMode.Fullscreen,
                ResolutionWidth = 1920,
                ResolutionHeight = 1080,
                VSync = true,
                TargetFrameRate = 60,
                QualityLevel = 1
            };

            feedbackController.RunSaveOperation(
                () =>
                {
                    runtime.ApplySettings(defaults);
                    RefreshFromSettings();
                });
        }

        private void OnPresetChanged(int index)
        {
            if (isRefreshing || index < 0 || index > 4)
                return;

            if (index == (int)VideoPreset.Custom)
                return;

            VideoSettingsData data = CreatePreset((VideoPreset)index);

            isRefreshing = true;

            SetDropdownWithoutNotify(
                windowModeDropdown,
                (int)data.WindowMode);

            SetDropdownWithoutNotify(
                resolutionDropdown,
                FindResolutionIndex(
                    data.ResolutionWidth,
                    data.ResolutionHeight));

            SetToggleWithoutNotify(vSyncToggle, data.VSync);

            SetDropdownWithoutNotify(
                frameRateDropdown,
                FindFrameRateIndex(data.TargetFrameRate));

            SetDropdownWithoutNotify(
                qualityDropdown,
                QualityLevelToDropdownIndex(data.QualityLevel));

            isRefreshing = false;
        }

        private void OnControlChanged(int value)
        {
            MarkCustomPreset();
        }

        private void OnToggleChanged(bool value)
        {
            MarkCustomPreset();
        }

        private void MarkCustomPreset()
        {
            if (isRefreshing || presetDropdown == null)
                return;

            isRefreshing = true;
            presetDropdown.SetValueWithoutNotify(
                (int)VideoPreset.Custom);
            isRefreshing = false;
        }

        private VideoSettingsData ReadControls()
        {
            VideoSettingsData current = runtime.Settings;

            VideoSettingsData data = new VideoSettingsData
            {
                WindowMode = windowModeDropdown != null
                    ? (VideoWindowMode)windowModeDropdown.value
                    : current.WindowMode,

                ResolutionWidth = current.ResolutionWidth,
                ResolutionHeight = current.ResolutionHeight,

                VSync = vSyncToggle != null
                    ? vSyncToggle.isOn
                    : current.VSync,

                TargetFrameRate = current.TargetFrameRate,
                QualityLevel = current.QualityLevel,

                Preset = presetDropdown != null
                    ? (VideoPreset)presetDropdown.value
                    : VideoPreset.Custom
            };

            if (resolutionDropdown != null &&
                resolutionDropdown.value >= 0 &&
                resolutionDropdown.value < Resolutions.Length)
            {
                Vector2Int resolution =
                    Resolutions[resolutionDropdown.value];

                data.ResolutionWidth = resolution.x;
                data.ResolutionHeight = resolution.y;
            }

            if (frameRateDropdown != null &&
                frameRateDropdown.value >= 0 &&
                frameRateDropdown.value < FrameRates.Length)
            {
                data.TargetFrameRate =
                    FrameRates[frameRateDropdown.value];
            }

            if (qualityDropdown != null)
            {
                data.QualityLevel =
                    DropdownIndexToQualityLevel(
                        qualityDropdown.value);
            }

            return data;
        }

        private VideoPreset DeterminePreset(VideoSettingsData data)
        {
            for (int i = 0; i < 4; i++)
            {
                VideoSettingsData preset = CreatePreset((VideoPreset)i);

                if (data.ResolutionWidth == preset.ResolutionWidth &&
                    data.ResolutionHeight == preset.ResolutionHeight &&
                    data.VSync == preset.VSync &&
                    data.TargetFrameRate == preset.TargetFrameRate &&
                    data.QualityLevel == preset.QualityLevel)
                {
                    return (VideoPreset)i;
                }
            }

            return VideoPreset.Custom;
        }

        private VideoSettingsData CreatePreset(VideoPreset preset)
        {
            VideoSettingsData data = new VideoSettingsData
            {
                WindowMode = runtime.Settings.WindowMode,
                Preset = preset
            };

            switch (preset)
            {
                case VideoPreset.Low:
                    data.ResolutionWidth = 1280;
                    data.ResolutionHeight = 720;
                    data.VSync = false;
                    data.TargetFrameRate = 60;
                    data.QualityLevel = 0;
                    break;

                case VideoPreset.Balanced:
                    data.ResolutionWidth = 1920;
                    data.ResolutionHeight = 1080;
                    data.VSync = true;
                    data.TargetFrameRate = 60;
                    data.QualityLevel = 1;
                    break;

                case VideoPreset.High:
                    data.ResolutionWidth = 1920;
                    data.ResolutionHeight = 1080;
                    data.VSync = true;
                    data.TargetFrameRate = 120;
                    data.QualityLevel = 2;
                    break;

                default:
                    data.ResolutionWidth = 1920;
                    data.ResolutionHeight = 1080;
                    data.VSync = true;
                    data.TargetFrameRate = 144;
                    data.QualityLevel = 3;
                    break;
            }

            return data;
        }

        private int QualityLevelToDropdownIndex(int qualityLevel)
        {
            return Mathf.Clamp(qualityLevel, 0, 3);
        }

        private int DropdownIndexToQualityLevel(int dropdownIndex)
        {
            return Mathf.Clamp(dropdownIndex, 0, 3);
        }

        private int FindResolutionIndex(int width, int height)
        {
            for (int i = 0; i < Resolutions.Length; i++)
            {
                if (Resolutions[i].x == width &&
                    Resolutions[i].y == height)
                {
                    return i;
                }
            }

            return 0;
        }

        private int FindFrameRateIndex(int frameRate)
        {
            for (int i = 0; i < FrameRates.Length; i++)
            {
                if (FrameRates[i] == frameRate)
                    return i;
            }

            return 1;
        }

        private void SetOptions(
            TMP_Dropdown dropdown,
            string[] options)
        {
            if (dropdown == null)
                return;

            dropdown.ClearOptions();
            dropdown.AddOptions(new List<string>(options));
        }

        private void SetDropdownWithoutNotify(
            TMP_Dropdown dropdown,
            int value)
        {
            if (dropdown == null || dropdown.options.Count == 0)
                return;

            dropdown.SetValueWithoutNotify(
                Mathf.Clamp(value, 0, dropdown.options.Count - 1));

            dropdown.RefreshShownValue();
        }

        private void SetToggleWithoutNotify(
            Toggle toggle,
            bool value)
        {
            if (toggle != null)
                toggle.SetIsOnWithoutNotify(value);
        }

        private bool EnsureRuntime()
        {
            if (runtime != null && runtime.IsInitialized)
                return true;

            ResolveRuntime();

            if (runtime != null && runtime.IsInitialized)
                return true;

            Debug.LogError(
                "[VideoSettingsUI] VideoSettingsRuntime " +
                "could not be resolved.",
                this);

            return false;
        }

        private SaveFeedbackController ResolveSaveFeedbackController()
        {
            BootstrapContext context = BootstrapContext.Current;

            if (context == null)
                return null;

            return context.GetComponentInChildren<SaveFeedbackController>(true);
        }
    }
}