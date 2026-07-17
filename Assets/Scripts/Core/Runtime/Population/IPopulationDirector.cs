namespace Chaosbound.Runtime.Population
{
    public interface IPopulationDirector
    {
        PopulationIntent Evaluate(
            PopulationContext context);
    }
}