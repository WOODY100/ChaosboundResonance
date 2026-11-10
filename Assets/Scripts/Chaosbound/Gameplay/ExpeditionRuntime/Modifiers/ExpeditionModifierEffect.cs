using System;

namespace Chaosbound.Gameplay.ExpeditionRuntime.Modifiers
{
    /// <summary>
    /// A concrete runtime effect. Value is a fractional percentage
    /// (for example, 0.20 represents +20%).
    /// </summary>
    public sealed class ExpeditionModifierEffect
    {
        public ExpeditionModifierTarget Target { get; }
        public string StatId { get; }
        public ModifierType Operation { get; }
        public float Value { get; }

        // Compatibility accessor for the existing EnemyRuntimeStats consumer.
        public float Percent => Value;

        public ExpeditionModifierEffect(
            ExpeditionModifierTarget target,
            string statId,
            float percent)
            : this(target, statId, ModifierType.Percent, percent)
        {
        }

        public ExpeditionModifierEffect(
            ExpeditionModifierTarget target,
            string statId,
            ModifierType operation,
            float value)
        {
            if (string.IsNullOrWhiteSpace(statId))
                throw new ArgumentException("StatId cannot be empty.", nameof(statId));

            if (target == ExpeditionModifierTarget.Global)
                throw new ArgumentException("Target.Global is reserved in V1.", nameof(target));

            if (operation != ModifierType.Percent)
                throw new ArgumentException("Only ModifierType.Percent is supported in V1.", nameof(operation));

            Target = target;
            StatId = statId;
            Operation = operation;
            Value = value;
        }

        public ExpeditionModifierEffect WithValue(float value)
        {
            return new ExpeditionModifierEffect(Target, StatId, Operation, value);
        }
    }
}
