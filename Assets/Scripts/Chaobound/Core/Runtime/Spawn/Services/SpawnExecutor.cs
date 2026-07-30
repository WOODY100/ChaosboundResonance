using Chaosbound.Core.Runtime.Spawn.Contracts;
using Chaosbound.Core.Runtime.Spawn.Domain;
using Chaosbound.Core.Runtime.Spawn.Results;
using Chaosbound.Core.Runtime.Spawn.Definitions;
using Chaosbound.Shared.Contracts;
using System;
using System.Collections.Generic;

namespace Chaosbound.Core.Runtime.Spawn.Services
{
    /// <summary>
    /// Executes SpawnJobs by resolving declarative content and
    /// materializing the requested runtime instances.
    /// </summary>
    public sealed class SpawnExecutor
    {
        private readonly IMaterializableResolver resolver;
        private readonly IMaterializer materializer;

        public SpawnExecutor(
            IMaterializableResolver resolver,
            IMaterializer materializer)
        {
            this.resolver = resolver
                ?? throw new ArgumentNullException(nameof(resolver));

            this.materializer = materializer
                ?? throw new ArgumentNullException(nameof(materializer));
        }

        public SpawnExecutionResult Execute(SpawnJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            IDefinition definition =
                resolver.ResolveDefinition(job.Materializable.Reference);

            int quantity = job.Quantity.Minimum;

            List<IMaterializedInstance> instances =
                new List<IMaterializedInstance>(quantity);

            for (int i = 0; i < quantity; i++)
            {
                IMaterializedInstance instance =
                    materializer.Materialize(definition);

                instances.Add(instance);
            }

            return new SpawnExecutionResult(instances);
        }
    }
}