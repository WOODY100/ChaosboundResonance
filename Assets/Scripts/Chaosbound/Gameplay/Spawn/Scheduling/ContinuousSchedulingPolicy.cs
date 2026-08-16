using Chaosbound.Gameplay.Spawn.Calculators;
using Chaosbound.Gameplay.Spawn.Domain;
using Chaosbound.Gameplay.Spawn.Factories;
using Chaosbound.Gameplay.Spawn.Models;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Spawn.Scheduling
{
    /// <summary>
    /// Schedules SpawnJobs using a continuous distribution strategy.
    /// </summary>
    public sealed class ContinuousSchedulingPolicy :
        ISpawnSchedulingPolicy
    {
        /// <summary>
        /// Temporary spawn interval.
        /// </summary>
        private static readonly TimeSpan DefaultSpawnInterval =
            TimeSpan.FromMilliseconds(500);

        private readonly SpawnBatchCalculator batchCalculator;

        private readonly SpawnTaskEntryFactory taskEntryFactory;

        private readonly SpawnTaskFactory taskFactory;

        public ContinuousSchedulingPolicy(
            SpawnBatchCalculator batchCalculator,
            SpawnTaskEntryFactory taskEntryFactory,
            SpawnTaskFactory taskFactory)
        {
            this.batchCalculator =
                batchCalculator
                ?? throw new ArgumentNullException(
                    nameof(batchCalculator));

            this.taskEntryFactory =
                taskEntryFactory
                ?? throw new ArgumentNullException(
                    nameof(taskEntryFactory));

            this.taskFactory =
                taskFactory
                ?? throw new ArgumentNullException(
                    nameof(taskFactory));
        }

        public IReadOnlyList<ScheduledSpawnTask> Schedule(
            SpawnSchedulingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            List<ScheduledSpawnTask> scheduledTasks =
                new();

            SpawnExecutionPlanEntry entry =
                context.Job.Entry;

            IReadOnlyList<int> batches =
                batchCalculator.Calculate(entry);

            int batchIndex = 0;

            foreach (int quantity in batches)
            {
                SpawnTaskEntry taskEntry =
                    taskEntryFactory.Create(
                        entry,
                        quantity);

                SpawnTask task =
                    taskFactory.Create(taskEntry);

                scheduledTasks.Add(
                    new ScheduledSpawnTask(
                        task,
                        TimeSpan.FromTicks(
                            DefaultSpawnInterval.Ticks *
                            batchIndex)));

                batchIndex++;
            }

            return scheduledTasks;
        }
    }
}