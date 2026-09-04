using System;
using System.Collections.Generic;

public sealed class SkillEvolutionPresentationData
{
    public RuntimeSkill CurrentSkill { get; }

    public int SlotIndex { get; }

    public IReadOnlyList<SkillEvolutionChoice> Choices { get; }

    public SkillEvolutionPresentationData(
        RuntimeSkill currentSkill,
        int slotIndex,
        IReadOnlyList<SkillEvolutionChoice> choices)
    {
        CurrentSkill =
            currentSkill
            ?? throw new ArgumentNullException(
                nameof(currentSkill));

        if (slotIndex < 0)
            throw new ArgumentOutOfRangeException(
                nameof(slotIndex));

        SlotIndex =
            slotIndex;

        Choices =
            choices
            ?? throw new ArgumentNullException(
                nameof(choices));
    }
}