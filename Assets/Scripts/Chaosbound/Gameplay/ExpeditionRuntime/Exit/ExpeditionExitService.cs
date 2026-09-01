using Chaosbound.Core.GameFlow;
using Chaosbound.Core.Runtime.SceneManagement;
using Chaosbound.Gameplay.ExpeditionRuntime.Director;
using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Exit
{
    /// <summary>
    /// Coordinates the terminal exit of an active Expedition.
    ///
    /// This service aborts the Expedition runtime,
    /// resets GameFlow and transitions to Sanctuary.
    ///
    /// It does not own Expedition runtime state,
    /// cleanup logic or GameFlow state.
    /// </summary>
    public sealed class ExpeditionExitService
    {
        private readonly ExpeditionDirector expeditionDirector;
        private readonly GameFlow gameFlow;
        private readonly SceneTransitionService sceneTransitionService;

        public ExpeditionExitService(
            ExpeditionDirector expeditionDirector,
            GameFlow gameFlow,
            SceneTransitionService sceneTransitionService)
        {
            this.expeditionDirector =
                expeditionDirector
                ?? throw new ArgumentNullException(
                    nameof(expeditionDirector));

            this.gameFlow =
                gameFlow
                ?? throw new ArgumentNullException(
                    nameof(gameFlow));

            this.sceneTransitionService =
                sceneTransitionService
                ?? throw new ArgumentNullException(
                    nameof(sceneTransitionService));
        }

        public void Exit(
            ExpeditionExitReason reason)
        {
            AbortExpedition();

            gameFlow.ResetFlow();

            sceneTransitionService.LoadScene(
                GameScene.Sanctuary);

            gameFlow.Initialize();
        }

        private void AbortExpedition()
        {
            expeditionDirector.AbortExpedition();
        }
    }
}