namespace Chaosbound.Content.Expeditions.Runtime.Combat.SpawnPattern
{
    public sealed class RuntimeSpawnPatternProfile
    {
        public float PerimeterPercentage { get; }

        public float FrontPercentage { get; }

        public float RearPercentage { get; }

        public float FlankPercentage { get; }

        public RuntimeSpawnPatternProfile(
            float perimeterPercentage,
            float frontPercentage,
            float rearPercentage,
            float flankPercentage)
        {
            PerimeterPercentage =
                perimeterPercentage;

            FrontPercentage =
                frontPercentage;

            RearPercentage =
                rearPercentage;

            FlankPercentage =
                flankPercentage;
        }
    }
}