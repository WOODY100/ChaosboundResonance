using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers
{
    public sealed class ExpeditionModifierEffect
    {
        public ExpeditionModifierTarget Target { get; }

        public string StatId { get; }

        public float Percent { get; }

        public ExpeditionModifierEffect(
            ExpeditionModifierTarget target,
            string statId,
            float percent)
        {
            if (string.IsNullOrWhiteSpace(statId))
                throw new ArgumentException(
                    "StatId cannot be empty.",
                    nameof(statId));

            Target = target;
            StatId = statId;
            Percent = percent;
        }
    }
}