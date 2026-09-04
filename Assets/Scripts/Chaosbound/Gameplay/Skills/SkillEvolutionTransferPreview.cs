using System;
using System.Collections.Generic;

public sealed class SkillEvolutionTransferPreview
{
    public IReadOnlyList<SkillModifierDefinition> RetainedModifiers { get; }
    public IReadOnlyList<SkillModifierDefinition> DroppedModifiers { get; }

    public SkillEvolutionTransferPreview(
        IReadOnlyList<SkillModifierDefinition> retainedModifiers,
        IReadOnlyList<SkillModifierDefinition> droppedModifiers)
    {
        RetainedModifiers =
            retainedModifiers ?? throw new ArgumentNullException(nameof(retainedModifiers));

        DroppedModifiers =
            droppedModifiers ?? throw new ArgumentNullException(nameof(droppedModifiers));
    }
}