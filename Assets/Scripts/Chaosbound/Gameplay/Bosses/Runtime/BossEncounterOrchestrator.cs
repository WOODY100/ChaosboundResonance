using System;
using System.Collections.Generic;
using UnityEngine;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Gameplay.Spawn.Runtime;

namespace Chaosbound.Gameplay.Bosses
{
    /// <summary>
    /// Prepares a complete Boss or MiniBoss encounter before allowing
    /// any participant to enter combat.
    /// The service is stateless and can be shared by both domains.
    /// </summary>
    public sealed class BossEncounterOrchestrator
    {
        private readonly SpawnRuntime spawnRuntime;

        public BossEncounterOrchestrator(SpawnRuntime spawnRuntime)
        {
            this.spawnRuntime = spawnRuntime
                ?? throw new ArgumentNullException(nameof(spawnRuntime));
        }

        public bool PrepareAndStart(
            IReadOnlyList<GameObject> participants,
            out string error)
        {
            error = null;

            if (participants == null || participants.Count == 0)
            {
                error = "The encounter contains no materialized participants.";
                return false;
            }

            HashSet<GameObject> uniqueParticipants = new HashSet<GameObject>();

            // Preparation barrier: every participant must be Ready first.
            for (int i = 0; i < participants.Count; i++)
            {
                GameObject participant = participants[i];

                if (participant == null || !uniqueParticipants.Add(participant))
                {
                    error = "The encounter contains a null or duplicate participant.";
                    Cleanup(participants);
                    return false;
                }

                BossRuntimeContext context =
                    participant.GetComponent<BossRuntimeContext>();
                BossCombatRuntime combat =
                    participant.GetComponent<BossCombatRuntime>();

                if (context == null || !context.IsInitialized)
                {
                    error = $"'{participant.name}' has no initialized BossRuntimeContext.";
                    Cleanup(participants);
                    return false;
                }

                if (combat == null)
                {
                    error = $"'{participant.name}' is missing BossCombatRuntime.";
                    Cleanup(participants);
                    return false;
                }

                if (!RegisterExecutors(participant, combat, out error))
                {
                    Cleanup(participants);
                    return false;
                }

                if (!combat.Initialize())
                {
                    error = $"'{participant.name}' could not prepare its combat runtime.";
                    Cleanup(participants);
                    return false;
                }

                if (combat.LifecycleState != BossCombatLifecycleState.Ready)
                {
                    error = $"'{participant.name}' did not reach the Ready state.";
                    Cleanup(participants);
                    return false;
                }
            }

            // Bind return-to-pool callbacks only after all participants are Ready.
            // No participant can die from combat before the start barrier.
            for (int i = 0; i < participants.Count; i++)
            {
                GameObject participant = participants[i];
                BossRuntimeLifecycle lifecycle =
                    participant.GetComponent<BossRuntimeLifecycle>();

                if (lifecycle == null)
                {
                    error =
                        $"'{participant.name}' is missing BossRuntimeLifecycle.";
                    Cleanup(participants);
                    return false;
                }

                lifecycle.BindDeathPresentationCompletion(
                    HandleDeathPresentationCompleted);
            }

            // Start barrier: combat begins only after every participant is Ready
            // and every lifecycle has a valid pool-return callback.
            for (int i = 0; i < participants.Count; i++)
            {
                BossCombatRuntime combat =
                    participants[i].GetComponent<BossCombatRuntime>();

                if (combat == null || !combat.StartCombat())
                {
                    error = $"'{participants[i].name}' failed to enter Active combat.";
                    Cleanup(participants);
                    return false;
                }
            }

            return true;
        }

        private void HandleDeathPresentationCompleted(GameObject participant)
        {
            if (participant == null)
                return;

            UnregisterRuntimeReference(participant);

            // SpawnRuntime owns materialized-object tracking and pooling.
            // Release only this participant; other encounter objects and
            // unrelated spawn operations remain untouched.
            spawnRuntime.ReleaseMaterializedObjects(
                new GameObject[] { participant });
        }

        private static bool RegisterExecutors(
            GameObject participant,
            BossCombatRuntime combat,
            out string error)
        {
            error = null;

            MonoBehaviour[] components =
                participant.GetComponentsInChildren<MonoBehaviour>(true);

            for (int i = 0; i < components.Length; i++)
            {
                IBossAttackExecutor executor = components[i] as IBossAttackExecutor;
                if (executor == null)
                    continue;

                if (!combat.RegisterExecutor(executor))
                {
                    error =
                        $"'{participant.name}' has a duplicate or invalid Boss attack executor " +
                        $"on component '{components[i].GetType().Name}'.";
                    return false;
                }
            }

            return true;
        }

        private void Cleanup(IReadOnlyList<GameObject> participants)
        {
            if (participants == null)
                return;

            for (int i = 0; i < participants.Count; i++)
            {
                GameObject participant = participants[i];
                if (participant == null)
                    continue;

                BossCombatRuntime combat =
                    participant.GetComponent<BossCombatRuntime>();

                if (combat != null)
                    combat.StopCombat();

                UnregisterRuntimeReference(participant);
            }

            // SpawnRuntime owns materialized-object tracking and pooling.
            // Release only this encounter's participants.
            spawnRuntime.ReleaseMaterializedObjects(participants);
        }

        private static void UnregisterRuntimeReference(GameObject participant)
        {
            if (participant == null)
                return;

            BossRuntimeContext context =
                participant.GetComponent<BossRuntimeContext>();

            if (context == null || !context.IsInitialized ||
                context.ExpeditionRuntime == null || context.Content == null)
                return;

            string domainId = context.Boss != null ? "boss" : "miniboss";

            context.ExpeditionRuntime.RuntimeReferences.Unregister(
                domainId,
                context.Content.Id);
        }
    }
}
