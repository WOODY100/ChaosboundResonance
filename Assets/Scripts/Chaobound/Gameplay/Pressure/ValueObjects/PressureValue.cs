namespace Chaosbound.Gameplay.Pressure.ValueObjects
{
    /// <summary>
    /// Represents the current pressure of an expedition.
    /// </summary>
    public readonly struct PressureValue
    {
        public float Value { get; }

        public PressureValue(float value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString("0.##");
        }
    }
}