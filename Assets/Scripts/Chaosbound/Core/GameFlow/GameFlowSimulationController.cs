using System;

namespace Chaosbound.Core.GameFlow
{
    public sealed class GameFlowSimulationController
    {
        public bool IsSimulationEnabled
        {
            get;
            private set;
        }

        public void Apply(
            bool simulationEnabled)
        {
            IsSimulationEnabled =
                simulationEnabled;

            UnityEngine.Time.timeScale =
                simulationEnabled
                    ? 1f
                    : 0f;
        }

        public void Reset()
        {
            IsSimulationEnabled =
                false;

            UnityEngine.Time.timeScale =
                0f;
        }
    }
}