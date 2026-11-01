using Chaosbound.Content.Enemy.Bosses;
using Chaosbound.Content.Enemy.Contracts;
using Chaosbound.Content.Enemy.MiniBosses;
using Chaosbound.Gameplay.ExpeditionRuntime.Runtime;
using Chaosbound.Shared.Identifiers;
using System;
using UnityEngine;

namespace Chaosbound.Gameplay.Bosses
{
    [RequireComponent(typeof(BossHealth))]
    public sealed class BossRuntimeContext : MonoBehaviour
    {
        private const string PlayerDomainId = "player";
        private static readonly ContentId PlayerContentId = new ContentId("player");

        private BossHealth health;

        public IBossCombatContent Content { get; private set; }
        public BossData Boss => Content as BossData;
        public MiniBossData MiniBoss => Content as MiniBossData;
        public BossCombatDefinition CombatDefinition =>
            Content != null ? Content.CombatDefinition : null;
        public ExpeditionRuntimeState ExpeditionRuntime { get; private set; }
        public Transform PlayerTarget { get; private set; }
        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            health = GetComponent<BossHealth>();
        }

        public void Initialize(
            IBossCombatContent content,
            ExpeditionRuntimeState expeditionRuntime)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));
            if (expeditionRuntime == null)
                throw new ArgumentNullException(nameof(expeditionRuntime));
            if (content.CombatDefinition == null)
                throw new InvalidOperationException(
                    $"CombatDefinition is missing for content '{content.DisplayName}'.");
            if (!content.CombatDefinition.Validate(out string error))
                throw new InvalidOperationException(
                    $"CombatDefinition for '{content.DisplayName}' is invalid: {error}");

            if (!expeditionRuntime.RuntimeReferences.TryResolve(
                PlayerDomainId,
                PlayerContentId,
                out Transform player))
            {
                throw new InvalidOperationException(
                    "The expedition Player reference is not registered.");
            }

            if (health == null)
                health = GetComponent<BossHealth>();
            if (health == null)
                throw new InvalidOperationException(
                    "Boss prefab is missing BossHealth.");

            health.InitializeForRuntime(content.CombatDefinition.BaseStats.MaxHP);

            Content = content;
            ExpeditionRuntime = expeditionRuntime;
            PlayerTarget = player;
            IsInitialized = true;
        }

        private void OnDisable()
        {
            Content = null;
            ExpeditionRuntime = null;
            PlayerTarget = null;
            IsInitialized = false;
        }
    }
}
