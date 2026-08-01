namespace Chaosbound.Gameplay.Threat.ValueObjects
{
    /// <summary>
    /// Represents the threat cost of an enemy.
    /// </summary>
    public readonly struct ThreatCost
    {
        public float Value { get; }

        public ThreatCost(float value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString("0.##");
        }
    }
}