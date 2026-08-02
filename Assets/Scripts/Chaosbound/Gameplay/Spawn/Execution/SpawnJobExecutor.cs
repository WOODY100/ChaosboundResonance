using Chaosbound.Gameplay.Spawn.Materialization;
using Chaosbound.Gameplay.Spawn.Models;
using Chaosbound.Gameplay.Spawn.Runtime;
using Chaosbound.Gameplay.Spawn.Scheduling;
using System;
using System.Collections.Generic;

namespace Chaosbound.Gameplay.Spawn.Execution
{
    /// <summary>
    /// Executes scheduled SpawnJobs.
    /// </summary>
    public sealed class SpawnJobExecutor
    {
        private readonly EnemyScheduler scheduler;

        private readonly SpawnJobRuntimeStateFactory
            runtimeStateFactory;

        private readonly ScheduledSpawnTaskExecutor
            taskExecutor;

        public SpawnJobExecutor(
            EnemyScheduler scheduler,
            SpawnJobRuntimeStateFactory runtimeStateFactory,
            ScheduledSpawnTaskExecutor taskExecutor)
        {
            this.scheduler =
                scheduler
                ?? throw new ArgumentNullException(nameof(scheduler));

            this.runtimeStateFactory =
                runtimeStateFactory
                ?? throw new ArgumentNullException(nameof(runtimeStateFactory));

            this.taskExecutor =
                taskExecutor
                ?? throw new ArgumentNullException(nameof(taskExecutor));
        }

        /// <summary>
        /// Executes the supplied scheduling context.
        /// </summary>
        public void Execute(
            EnemySchedulingContext schedulingContext)
        {
            if (schedulingContext == null)
                throw new ArgumentNullException(nameof(schedulingContext));

            IReadOnlyList<ScheduledSpawnTask> tasks =
                scheduler.Schedule(
                    schedulingContext);

            SpawnJobRuntimeState runtimeState =
                runtimeStateFactory.Create(
                    schedulingContext.Job);

            foreach (ScheduledSpawnTask task in tasks)
            {
                taskExecutor.Execute(
                    task,
                    runtimeState);
            }
        }
    }
}