using System;

public sealed class SkillEvolutionChoice
{
    public SkillEvolutionDefinition Evolution { get; }

    public SkillEvolutionTransferPreview TransferPreview { get; }

    public SkillEvolutionChoice(
        SkillEvolutionDefinition evolution,
        SkillEvolutionTransferPreview transferPreview)
    {
        Evolution =
            evolution
            ?? throw new ArgumentNullException(
                nameof(evolution));

        TransferPreview =
            transferPreview
            ?? throw new ArgumentNullException(
                nameof(transferPreview));
    }
}