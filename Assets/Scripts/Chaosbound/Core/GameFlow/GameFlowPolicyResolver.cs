namespace Chaosbound.Core.GameFlow
{
    public sealed class GameFlowPolicyResolver
    {
        private readonly GameFlowConfiguration configuration;

        public GameFlowPolicyResolver(
            GameFlowConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public GameFlowPolicy Resolve(
            GameFlowContext context)
        {
            switch (context)
            {
                case GameFlowContext.Playing:
                    return new GameFlowPolicy(
                        simulation: true,
                        gameplay: true,
                        gameplayInput: true,
                        uiInput: true);

                case GameFlowContext.Pause:
                    return new GameFlowPolicy(
                        simulation: false,
                        gameplay: false,
                        gameplayInput: false,
                        uiInput: true);

                case GameFlowContext.Confirmation:
                    return new GameFlowPolicy(
                        simulation: configuration != null
                            ? configuration.ConfirmationSimulation
                            : true,
                        gameplay: false,
                        gameplayInput: false,
                        uiInput: true);

                case GameFlowContext.LevelUp:
                    return new GameFlowPolicy(
                        simulation: false,
                        gameplay: false,
                        gameplayInput: false,
                        uiInput: true);

                case GameFlowContext.Inventory:
                    return new GameFlowPolicy(
                        simulation: false,
                        gameplay: false,
                        gameplayInput: false,
                        uiInput: true);

                case GameFlowContext.Stats:
                    return new GameFlowPolicy(
                        simulation: false,
                        gameplay: false,
                        gameplayInput: false,
                        uiInput: true);

                case GameFlowContext.Dialogue:
                    return new GameFlowPolicy(
                        simulation: configuration != null
                            ? configuration.DialogueSimulation
                            : false,
                        gameplay: false,
                        gameplayInput: false,
                        uiInput: true);

                case GameFlowContext.GameOver:
                    return new GameFlowPolicy(
                        simulation: false,
                        gameplay: false,
                        gameplayInput: false,
                        uiInput: true);

                default:
                    return GetNeutralPolicy();
            }
        }

        private GameFlowPolicy GetNeutralPolicy()
        {
            return new GameFlowPolicy(
                simulation: false,
                gameplay: false,
                gameplayInput: false,
                uiInput: false);
        }
    }
}