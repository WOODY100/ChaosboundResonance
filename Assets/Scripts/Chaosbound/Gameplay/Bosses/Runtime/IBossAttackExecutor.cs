using System;
using Chaosbound.Content.Enemy.Bosses;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Adapter for one exact concrete BossAttackEffectDefinition type.
    /// Implementations must not share mutable execution state between Bosses.
    /// </summary>
    public interface IBossAttackExecutor
    {
        Type EffectDefinitionType { get; }

        bool CanExecute(BossAttackEffectDefinition definition);

        bool TryCreateExecution(
            BossAttackExecutionContext context,
            out IBossAttackExecution execution);

        BossAttackMaterializationResult TryRelease(
            BossAttackExecutionContext executionContext,
            BossAttackReleaseContext releaseContext,
            int releaseIndex);
    }
}
