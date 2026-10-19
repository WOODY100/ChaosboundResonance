
using System;
using UnityEngine;

namespace Chaosbound.Core.Settings
{
    public sealed class VideoSettingsRuntime : MonoBehaviour
    {
        private VideoSettingsData settings;
        private VideoSettingsData previousSettings;
        private VideoSettingsPersistenceService persistenceService;

        public VideoSettingsData Settings => settings;
        public bool IsInitialized => settings != null;

        public bool HasPendingPreview => previousSettings != null;

        public void Initialize(
            VideoSettingsPersistenceService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (IsInitialized)
            {
                return;
            }

            persistenceService = service;
            settings = CloneSettings(
                persistenceService.LoadOrCreateDefaults());

            ApplyToUnity(settings);
        }

        /// <summary>
        /// Aplica y guarda inmediatamente una configuración.
        /// Se conserva para operaciones que no requieren confirmación.
        /// </summary>
        public void ApplySettings(VideoSettingsData newSettings)
        {
            if (newSettings == null)
            {
                throw new ArgumentNullException(nameof(newSettings));
            }

            // Si existía una prueba pendiente, se cancela antes de
            // establecer una configuración definitiva.
            previousSettings = null;

            VideoSettingsData copy = CloneSettings(newSettings);

            ApplyToUnity(copy);
            SaveSettings(copy);
        }

        /// <summary>
        /// Aplica temporalmente una configuración sin guardarla.
        /// Conserva una copia de la configuración anterior.
        /// </summary>
        public bool TryApplySettings(VideoSettingsData newSettings)
        {
            if (newSettings == null)
            {
                throw new ArgumentNullException(nameof(newSettings));
            }

            if (!IsInitialized)
            {
                Debug.LogError(
                    "[VideoSettingsRuntime] Runtime is not initialized.",
                    this);

                return false;
            }

            if (persistenceService == null)
            {
                Debug.LogError(
                    "[VideoSettingsRuntime] Persistence service " +
                    "is not initialized.",
                    this);

                return false;
            }

            // No permitimos reemplazar accidentalmente una prueba
            // que todavía está esperando la decisión del jugador.
            if (HasPendingPreview)
            {
                Debug.LogWarning(
                    "[VideoSettingsRuntime] A video settings preview " +
                    "is already pending.",
                    this);

                return false;
            }

            previousSettings = CloneSettings(settings);

            VideoSettingsData previewSettings =
                CloneSettings(newSettings);

            pendingPreviewSettings = CloneSettings(previewSettings);

            ApplyToUnity(previewSettings);

            // Todavía no modificamos settings ni el archivo guardado.
            return true;
        }

        /// <summary>
        /// Confirma la configuración temporal y la guarda.
        /// </summary>
        public bool ConfirmPreviewSettings()
        {
            if (!HasPendingPreview)
            {
                return false;
            }

            // La configuración que se probó es la que está aplicada
            // actualmente en Unity. La copia definitiva se mantiene
            // separada del snapshot anterior.
            VideoSettingsData confirmedSettings =
                CloneSettingsFromAppliedPreview();

            if (confirmedSettings == null)
            {
                return false;
            }

            if (persistenceService == null)
            {
                Debug.LogError(
                    "[VideoSettingsRuntime] Persistence service " +
                    "is not initialized.",
                    this);

                return false;
            }

            persistenceService.Save(confirmedSettings);

            settings = confirmedSettings;
            previousSettings = null;

            pendingPreviewSettings = null;

            return true;
        }

        /// <summary>
        /// Descarta la prueba y recupera la configuración anterior.
        /// </summary>
        public bool RevertPreviewSettings()
        {
            if (!HasPendingPreview)
            {
                return false;
            }

            VideoSettingsData restoreSettings =
                CloneSettings(previousSettings);

            previousSettings = null;

            pendingPreviewSettings = null;

            ApplyToUnity(restoreSettings);
            settings = restoreSettings;

            return true;
        }

        public void SaveSettings(VideoSettingsData newSettings)
        {
            if (newSettings == null)
            {
                throw new ArgumentNullException(nameof(newSettings));
            }

            if (persistenceService == null)
            {
                Debug.LogError(
                    "[VideoSettingsRuntime] Persistence service " +
                    "is not initialized.",
                    this);

                return;
            }

            VideoSettingsData copy = CloneSettings(newSettings);

            persistenceService.Save(copy);
            settings = copy;
        }

        public void ApplyCurrentSettings()
        {
            if (!IsInitialized)
            {
                Debug.LogError(
                    "[VideoSettingsRuntime] Runtime is not initialized.",
                    this);

                return;
            }

            ApplyToUnity(settings);
        }

        private VideoSettingsData CloneSettingsFromAppliedPreview()
        {
            // Durante la prueba, settings sigue representando la
            // configuración definitiva anterior. Por eso necesitamos
            // guardar explícitamente la configuración que se probó.
            return pendingPreviewSettings != null
                ? CloneSettings(pendingPreviewSettings)
                : null;
        }

        private VideoSettingsData pendingPreviewSettings;

        private static VideoSettingsData CloneSettings(
            VideoSettingsData source)
        {
            if (source == null)
            {
                return null;
            }

            return new VideoSettingsData
            {
                Preset = source.Preset,
                WindowMode = source.WindowMode,
                ResolutionWidth = source.ResolutionWidth,
                ResolutionHeight = source.ResolutionHeight,
                VSync = source.VSync,
                TargetFrameRate = source.TargetFrameRate,
                QualityLevel = source.QualityLevel
            };
        }

        private void ApplyToUnity(VideoSettingsData data)
        {
            if (data == null)
            {
                return;
            }

            QualitySettings.vSyncCount = data.VSync ? 1 : 0;

            Application.targetFrameRate =
                data.TargetFrameRate <= 0
                    ? -1
                    : data.TargetFrameRate;

            if (QualitySettings.names.Length > 0)
            {
                int qualityIndex = Mathf.Clamp(
                    data.QualityLevel,
                    0,
                    QualitySettings.names.Length - 1);

                QualitySettings.SetQualityLevel(
                    qualityIndex,
                    true);
            }

            FullScreenMode fullScreenMode;

            switch (data.WindowMode)
            {
                case VideoWindowMode.BorderlessWindow:
                    fullScreenMode =
                        FullScreenMode.FullScreenWindow;
                    break;

                case VideoWindowMode.Windowed:
                    fullScreenMode =
                        FullScreenMode.Windowed;
                    break;

                default:
                    fullScreenMode =
                        FullScreenMode.ExclusiveFullScreen;
                    break;
            }

            Screen.SetResolution(
                data.ResolutionWidth,
                data.ResolutionHeight,
                fullScreenMode);
        }
    }
}
