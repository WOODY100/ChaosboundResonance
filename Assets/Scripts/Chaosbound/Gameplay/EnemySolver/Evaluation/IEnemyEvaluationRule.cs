using Chaosbound.Gameplay.EnemySolver.Models;
namespace Chaosbound.Gameplay.EnemySolver.Evaluation
{
    /// <summary>
    /// Represents a single tactical evaluation rule used by the
    /// EnemySolver candidate evaluation pipeline.
    /// </summary>
    public interface IEnemyEvaluationRule
    {
        /// <summary>
        /// Evaluates the specified candidate within the current
        /// evaluation context.
        /// </summary>
        /// <param name="candidate">
        /// Candidate being evaluated.
        /// </param>
        /// <param name="context">
        /// Current evaluation context.
        /// </param>
        /// <returns>
        /// Score contribution produced by this rule.
        /// </returns>
        CandidateScore Evaluate(
            EnemyCandidate candidate,
            EvaluationContext context);
    }
}