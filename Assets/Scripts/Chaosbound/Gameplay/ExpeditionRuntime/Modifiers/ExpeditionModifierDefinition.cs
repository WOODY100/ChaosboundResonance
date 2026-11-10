using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers
{
    /// <summary>
    /// Declarative content definition for an expedition modifier.
    /// Runtime applications are created by ExpeditionModifierDomainDirector.
    /// </summary>
    [CreateAssetMenu(
        fileName = "ExpeditionModifierDefinition",
        menuName = "Chaosbound/Expedition/Modifier Definition")]
    public sealed class ExpeditionModifierDefinition : ScriptableObject
    {
        [SerializeField] private string contentId;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;
        [SerializeField] private List<ExpeditionModifierEffectConfiguration> effects =
            new List<ExpeditionModifierEffectConfiguration>();
        [SerializeField] private ExpeditionModifierStackingPolicy stackingPolicy =
            ExpeditionModifierStackingPolicy.Independent;
        [SerializeField, Min(1)] private int maxStacks = 1;
        [SerializeField, Min(0f)] private float durationSeconds;
        [SerializeField] private ExpeditionModifierLifetime lifetime =
            ExpeditionModifierLifetime.Expedition;

        public string ContentId => contentId;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public IReadOnlyList<ExpeditionModifierEffectConfiguration> Effects => effects;
        public ExpeditionModifierStackingPolicy StackingPolicy => stackingPolicy;
        public int MaxStacks => maxStacks;
        public TimeSpan BaseDuration => TimeSpan.FromSeconds(durationSeconds);
        public ExpeditionModifierLifetime Lifetime => lifetime;

        /// <summary>Validates the definition without mutating it.</summary>
        public bool TryValidate(out string error)
        {
            if (string.IsNullOrWhiteSpace(contentId))
            {
                error = "ContentId cannot be empty.";
                return false;
            }

            if (effects == null || effects.Count == 0)
            {
                error = "At least one modifier effect is required.";
                return false;
            }

            for (int i = 0; i < effects.Count; i++)
            {
                if (effects[i] == null)
                {
                    error = $"Modifier effect at index {i} is null.";
                    return false;
                }

                if (!effects[i].TryValidate(out error))
                    return false;
            }

            if (lifetime == ExpeditionModifierLifetime.Timed &&
                durationSeconds <= 0f)
            {
                error = "Timed modifiers require a positive duration.";
                return false;
            }

            if (lifetime == ExpeditionModifierLifetime.Expedition &&
                stackingPolicy != ExpeditionModifierStackingPolicy.Independent)
            {
                error = "Expedition lifetime only supports Independent stacking in V1.";
                return false;
            }

            if (stackingPolicy != ExpeditionModifierStackingPolicy.Independent &&
                maxStacks < 1)
            {
                error = "MaxStacks must be at least 1.";
                return false;
            }

            error = null;
            return true;
        }

        private void OnValidate()
        {
            maxStacks = Mathf.Max(1, maxStacks);
            durationSeconds = Mathf.Max(0f, durationSeconds);
            if (effects == null)
                effects = new List<ExpeditionModifierEffectConfiguration>();
        }
    }

    [Serializable]
    public sealed class ExpeditionModifierEffectConfiguration
    {
        [SerializeField] private ExpeditionModifierTarget target =
            ExpeditionModifierTarget.Player;
        [SerializeField] private string statId;
        [SerializeField] private ModifierType operation = ModifierType.Percent;
        [SerializeField] private float value;

        public ExpeditionModifierTarget Target => target;
        public string StatId => statId;
        public ModifierType Operation => operation;
        public float Value => value;

        public bool TryValidate(out string error)
        {
            if (target == ExpeditionModifierTarget.Global)
            {
                error = "Target.Global is reserved and is not supported in V1.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(statId))
            {
                error = "StatId cannot be empty.";
                return false;
            }

            if (operation != ModifierType.Percent)
            {
                error = "Only ModifierType.Percent is supported in V1.";
                return false;
            }

            error = null;
            return true;
        }
    }
}
