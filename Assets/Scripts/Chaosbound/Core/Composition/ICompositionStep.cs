namespace Chaosbound.Core.Composition
{
    /// <summary>
    /// Represents a single step of the world composition pipeline.
    /// </summary>
    public interface ICompositionStep
    {
        void Execute(CompositionContext context);
    }
}