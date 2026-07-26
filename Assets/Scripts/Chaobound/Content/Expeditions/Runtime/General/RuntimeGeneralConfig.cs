using Chaosbound.Content.Expeditions.Enums;

namespace Chaosbound.Content.Expeditions.Runtime.General
{ 
    /// <summary>
    /// Runtime configuration for an expedition. 
    /// </summary> 
    public sealed class RuntimeGeneralConfig 
    {
        public CompletionCondition CompletionCondition { get; } 
        public DifficultyTier BaseDifficulty { get; } 
        public RuntimeGeneralConfig( CompletionCondition completionCondition, DifficultyTier baseDifficulty) 
        { 
            CompletionCondition = completionCondition; BaseDifficulty = baseDifficulty; 
        } 
    } 
}