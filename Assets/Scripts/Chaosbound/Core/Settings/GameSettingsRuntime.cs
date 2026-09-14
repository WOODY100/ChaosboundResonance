using UnityEngine;

namespace Chaosbound.Core.Settings
{
    public sealed class GameSettingsRuntime : MonoBehaviour
    {
        private GameSettings settings;

        public GameSettings Settings =>
            settings;

        private void Awake()
        {
            settings = new GameSettings();
        }
    }
}