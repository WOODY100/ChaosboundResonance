using Chaosbound.Content.Expeditions;
using System;

namespace Chaosbound.Runtime.Run.Configs.General
{ 
    /// <summary>
    /// Runtime configuration for an expedition. 
    /// </summary> 
    public sealed class GeneralConfig 
    {
        public CompletionCondition CompletionCondition { get; } 
        public DifficultyTier BaseDifficulty { get; } 
        public GeneralConfig( CompletionCondition completionCondition, DifficultyTier baseDifficulty) 
        { 
            CompletionCondition = completionCondition; BaseDifficulty = baseDifficulty; 
        } 
    } 
}