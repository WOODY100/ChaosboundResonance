namespace Chaosbound.Core.GameFlow
{
    public static class GameFlowTransitionRules
    {
        public static GameFlowTransitionDecision Evaluate(
            GameFlowContext current,
            GameFlowContext requested)
        {
            //======================================================
            // Same Context
            //======================================================

            if (current == requested)
                return GameFlowTransitionDecision.Reject;

            //======================================================
            // GameOver
            //======================================================

            if (requested == GameFlowContext.GameOver)
                return GameFlowTransitionDecision.Replace;

            if (current == GameFlowContext.GameOver)
                return GameFlowTransitionDecision.Reject;

            //======================================================
            // Result
            //======================================================

            if (requested == GameFlowContext.Result)
                return GameFlowTransitionDecision.Replace;

            if (current == GameFlowContext.Result)
                return GameFlowTransitionDecision.Reject;

            //======================================================
            // LevelUp
            //======================================================

            if (requested == GameFlowContext.LevelUp)
            {
                if (current == GameFlowContext.Playing)
                    return GameFlowTransitionDecision.Push;

                return GameFlowTransitionDecision.Pending;
            }

            //======================================================
            // Evolution
            //======================================================

            if (requested == GameFlowContext.Evolution)
            {
                if (current == GameFlowContext.Playing)
                    return GameFlowTransitionDecision.Push;

                return GameFlowTransitionDecision.Pending;
            }

            //======================================================
            // Playing
            //======================================================

            if (requested == GameFlowContext.Playing)
                return GameFlowTransitionDecision.Reject;

            //======================================================
            // Pause
            //======================================================

            if (current == GameFlowContext.Playing &&
                requested == GameFlowContext.Pause)
            {
                return GameFlowTransitionDecision.Push;
            }

            //======================================================
            // Confirmation
            //======================================================

            if (requested == GameFlowContext.Confirmation)
            {
                switch (current)
                {
                    case GameFlowContext.Playing:
                    case GameFlowContext.Pause:
                    case GameFlowContext.Inventory:
                    case GameFlowContext.Stats:
                    case GameFlowContext.Dialogue:
                        return GameFlowTransitionDecision.Push;
                }
            }

            //======================================================
            // Inventory
            //======================================================

            if (current == GameFlowContext.Pause &&
                requested == GameFlowContext.Inventory)
            {
                return GameFlowTransitionDecision.Push;
            }

            if (current == GameFlowContext.Stats &&
                requested == GameFlowContext.Inventory)
            {
                return GameFlowTransitionDecision.Push;
            }

            //======================================================
            // Stats
            //======================================================

            if (current == GameFlowContext.Pause &&
                requested == GameFlowContext.Stats)
            {
                return GameFlowTransitionDecision.Push;
            }

            if (current == GameFlowContext.Inventory &&
                requested == GameFlowContext.Stats)
            {
                return GameFlowTransitionDecision.Push;
            }

            //======================================================
            // Dialogue
            //======================================================

            if (current == GameFlowContext.Playing &&
                requested == GameFlowContext.Dialogue)
            {
                return GameFlowTransitionDecision.Push;
            }

            //======================================================
            // Default
            //======================================================

            return GameFlowTransitionDecision.Reject;
        }
    }
}