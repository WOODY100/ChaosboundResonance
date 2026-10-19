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
                    settings.Vibration
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
                    Vibration
            };
        }
    }
}