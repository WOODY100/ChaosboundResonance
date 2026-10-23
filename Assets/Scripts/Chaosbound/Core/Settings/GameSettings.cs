namespace Chaosbound.Core.Settings
{
    public sealed class GameSettings
    {
        public bool ConfirmWorldItemDrop { get; set; } = true;

        public bool ShowDamageNumbers { get; set; } = true;

        public bool ShowEnemyHealthBars { get; set; } = true;

        public bool Vibration { get; set; } = true;

        // Audio
        public float MasterVolume { get; set; } = 0.8f;

        public float MusicVolume { get; set; } = 0.8f;

        public float EffectsVolume { get; set; } = 0.8f;

        public float UIVolume { get; set; } = 0.8f;

        public bool MuteAll { get; set; } = false;
    }
}