using System;

namespace Chaosbound.Core.Settings
{
    public enum VideoPreset
    {
        Low = 0,
        Balanced = 1,
        High = 2,
        Ultra = 3,
        Custom = 4
    }

    public enum VideoWindowMode
    {
        Fullscreen = 0,
        BorderlessWindow = 1,
        Windowed = 2
    }

    [Serializable]
    public sealed class VideoSettingsData
    {
        public VideoPreset Preset = VideoPreset.Balanced;

        public VideoWindowMode WindowMode =
            VideoWindowMode.Fullscreen;

        public int ResolutionWidth = 1920;
        public int ResolutionHeight = 1080;

        public bool VSync = true;

        // 30, 60, 120, 144 o 0 para ilimitado.
        public int TargetFrameRate = 60;

        // Índice de calidad de Unity.
        public int QualityLevel = 2;
    }
}