using System;

namespace Chaosbound.Core.Settings
{
    [Serializable]
    public sealed class GameSettingsData
    {
        public bool ConfirmWorldItemDrop = true;
        public bool ShowDamageNumbers = true;
        public bool ShowEnemyHealthBars = true;
        public bool Vibration = true;

        // Audio
        public float MasterVolume = 0.8f;
        public float MusicVolume = 0.8f;
        public float AmbienceVolume = 0.8f;
        public float EffectsVolume = 0.8f;
        public float UIVolume = 0.8f;
        public bool MuteAll = false;

        public static GameSettingsData FromSettings(
            GameSettings settings)
        {
            if (settings == null)
                settings = new GameSettings();

            return new GameSettingsData
            {
                ConfirmWorldItemDrop =
                    settings.ConfirmWorldItemDrop,

                ShowDamageNumbers =
                    settings.ShowDamageNumbers,

                ShowEnemyHealthBars =
                    settings.ShowEnemyHealthBars,

                Vibration =
                    settings.Vibration,

                MasterVolume =
                    settings.MasterVolume,

                MusicVolume =
                    settings.MusicVolume,

                AmbienceVolume =
                    settings.AmbienceVolume,

                EffectsVolume =
                    settings.EffectsVolume,

                UIVolume =
                    settings.UIVolume,

                MuteAll =
                    settings.MuteAll
            };
        }

        public GameSettings ToSettings()
        {
            return new GameSettings
            {
                ConfirmWorldItemDrop =
                    ConfirmWorldItemDrop,

                ShowDamageNumbers =
                    ShowDamageNumbers,

                ShowEnemyHealthBars =
                    ShowEnemyHealthBars,

                Vibration =
                    Vibration,

                MasterVolume =
                    MasterVolume,

                MusicVolume =
                    MusicVolume,

                AmbienceVolume =
                    AmbienceVolume,

                EffectsVolume =
                    EffectsVolume,

                UIVolume =
                    UIVolume,

                MuteAll =
                    MuteAll
            };
        }
    }
}