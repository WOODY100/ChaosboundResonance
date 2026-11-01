using UnityEngine;

namespace Chaosbound.Content.Enemy.Bosses
{
    /// <summary>
    /// Declarative base for effects materialized by Boss attack executors.
    /// Concrete effect families are implemented in the effects block.
    /// </summary>
    public abstract class BossAttackEffectDefinition : ScriptableObject
    {
    }
}
