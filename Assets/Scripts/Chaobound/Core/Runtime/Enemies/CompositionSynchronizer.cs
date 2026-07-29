using System;

namespace Chaosbound.Core.Runtime.Enemies
{
    /// <summary>
    /// Compares the desired enemy composition with the current
    /// materialized composition and produces the synchronization
    /// required to align both states.
    /// </summary>
    public sealed class CompositionSynchronizer
    {
        /// <summary>
        /// Produces the synchronization required to transform the
        /// current composition into the desired composition.
        /// </summary>
        public CompositionSynchronization Synchronize(
            EnemyComposition desiredComposition,
            CompositionState currentState)
        {
            if (desiredComposition == null)
            {
                throw new ArgumentNullException(nameof(desiredComposition));
            }

            if (currentState == null)
            {
                throw new ArgumentNullException(nameof(currentState));
            }

            CompositionSynchronization synchronization =
                new CompositionSynchronization();

            SynchronizeMissingEnemies(
                desiredComposition,
                currentState,
                synchronization);

            SynchronizeExcessEnemies(
                desiredComposition,
                currentState,
                synchronization);

            return synchronization;
        }

        private static void SynchronizeMissingEnemies(
            EnemyComposition desiredComposition,
            CompositionState currentState,
            CompositionSynchronization synchronization)
        {
            foreach (EnemyCompositionEntry desiredEntry in desiredComposition.Entries)
            {
                int currentAmount = 0;

                if (currentState.TryGetEntry(
                    desiredEntry.Variant,
                    out CompositionStateEntry currentEntry))
                {
                    currentAmount = currentEntry.AliveCount;
                }

                int missingAmount =
                    desiredEntry.Quantity - currentAmount;

                if (missingAmount <= 0)
                {
                    continue;
                }

                synchronization.Add(
                    new CompositionSynchronizationEntry(
                        desiredEntry.Variant,
                        SynchronizationOperationType.Spawn,
                        missingAmount));
            }
        }

        private static void SynchronizeExcessEnemies(
            EnemyComposition desiredComposition,
            CompositionState currentState,
            CompositionSynchronization synchronization)
        {
            foreach (CompositionStateEntry currentEntry in currentState.Entries)
            {
                int desiredAmount = 0;

                if (desiredComposition.TryGetEntry(
                    currentEntry.Variant,
                    out EnemyCompositionEntry desiredEntry))
                {
                    desiredAmount = desiredEntry.Quantity;
                }

                int excessAmount =
                    currentEntry.AliveCount - desiredAmount;

                if (excessAmount <= 0)
                {
                    continue;
                }

                synchronization.Add(
                    new CompositionSynchronizationEntry(
                        currentEntry.Variant,
                        SynchronizationOperationType.Despawn,
                        excessAmount));
            }
        }
    }
}