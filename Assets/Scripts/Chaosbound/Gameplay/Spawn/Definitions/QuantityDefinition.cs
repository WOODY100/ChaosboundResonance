using System;
using Chaosbound.Shared.Contracts;

namespace Chaosbound.Gameplay.Spawn.Definitions
{
    /// <summary>
    /// Describes the quantity range for a spawn job.
    /// </summary>
    public sealed class QuantityDefinition : IDefinition
    {
        public int Minimum { get; }

        public int Maximum { get; }

        public QuantityDefinition(int minimum, int maximum)
        {
            if (minimum < 0)
                throw new ArgumentOutOfRangeException(nameof(minimum));

            if (maximum < minimum)
                throw new ArgumentOutOfRangeException(nameof(maximum));

            Minimum = minimum;
            Maximum = maximum;
        }
    }
}