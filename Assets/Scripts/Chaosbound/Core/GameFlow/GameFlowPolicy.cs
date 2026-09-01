namespace Chaosbound.Core.GameFlow
{
    public readonly struct GameFlowPolicy
    {
        public bool Simulation { get; }
        public bool Gameplay { get; }
        public bool GameplayInput { get; }
        public bool UIInput { get; }

        public GameFlowPolicy(
            bool simulation,
            bool gameplay,
            bool gameplayInput,
            bool uiInput)
        {
            Simulation = simulation;
            Gameplay = gameplay;
            GameplayInput = gameplayInput;
            UIInput = uiInput;
        }
    }
}