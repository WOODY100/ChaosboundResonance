using System;

namespace Chaosbound.Gameplay.Bosses
{
    public interface IBossAttackExecution : IDisposable
    {
        BossAttackExecutionState State { get; }

        event Action<int> ReleaseRequested;

        /// <summary>
        /// Raised whenever the execution changes lifecycle state. Implementations
        /// must raise this after updating State and before issuing releases for that state.
        /// </summary>
        event Action<BossAttackExecutionState> StateChanged;

        bool Initialize(BossAttackExecutionContext context);

        void Tick(float deltaTime);

        void Cancel();
    }
}
