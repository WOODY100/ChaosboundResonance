using Chaosbound.Gameplay.ExpeditionRuntime.Completion.Contracts;
using Chaosbound.Gameplay.Spawn.Reference.Contracts;
using Chaosbound.Gameplay.Spawn.Reference.Models;
using Chaosbound.Shared.Identifiers;
using System;

namespace Chaosbound.Gameplay.Spawn.Reference.Providers
{
    /// <summary>
    /// Resolves the runtime reference associated
    /// with the origin that completed the expedition.
    /// </summary>
    public sealed class CompletionOriginReferenceProvider :
        ISpawnReferenceProvider
    {
        public SpawnReferenceResult Resolve(
            SpawnReferenceContext context)
        {
            if (context == null)
                throw new ArgumentNullException(
                    nameof(context));

            ExpeditionCompleted? completedExpedition =
                context
                    .ExpeditionRuntime
                    .Completion
                    .CompletedExpedition;

            if (!completedExpedition.HasValue)
            {
                return SpawnReferenceResult.Failure(
                    "Expedition completion origin is not available.");
            }

            CompletionOrigin origin =
                completedExpedition.Value.Origin;

            bool resolved =
                context
                    .ExpeditionRuntime
                    .RuntimeReferences
                    .TryResolve(
                        origin.DomainId,
                        origin.ContentId,
                        out UnityEngine.Transform transform);

            if (!resolved)
            {
                return SpawnReferenceResult.Failure(
                    $"No runtime reference was registered for " +
                    $"completion origin '{origin.DomainId}:{origin.ContentId}'.");
            }

            return SpawnReferenceResult.Success(
                transform);
        }
    }
}