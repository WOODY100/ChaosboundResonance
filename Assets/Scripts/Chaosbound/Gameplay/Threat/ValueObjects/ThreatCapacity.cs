namespace Chaosbound.Gameplay.Threat.ValueObjects
{
    /// <summary>
    /// Represents the maximum threat capacity available
    /// for the current state of the expedition.
    /// </summary>
    public readonly struct ThreatCapacity
    {
        public float Value { get; }

        public ThreatCapacity(float value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString("0.##");
        }
    }
}