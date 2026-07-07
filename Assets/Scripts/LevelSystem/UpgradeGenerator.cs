using UnityEngine;
using System.Collections.Generic;

public class UpgradeGenerator : MonoBehaviour
{
    [SerializeField] private SkillDatabase database;
    [SerializeField] private List<GlobalUpgradeDefinition> globalUpgrades;

    [Header("Weights")]
    [SerializeField] private float newSkillWeight = 1f;
    [SerializeField] private float modifierWeight = 2f;
    [SerializeField] private float globalModifierWeight = 1.5f;

    [Range(0f, 1f)]
    [SerializeField] private float newSkillChanceWhenFull = 0.25f;

    private readonly List<SkillDefinition> availableSkills = new();
    private readonly List<RuntimeSkill> ownedSkills = new();
    private readonly List<SkillModifierDefinition> filteredModifiers = new();
    private readonly List<GlobalUpgradeDefinition> filteredGlobals = new();

    public List<UpgradeOption> GenerateOptions(PlayerSkillLoadout loadout)
    {
        List<UpgradeOption> options = new List<UpgradeOption>();

        int safety = 0;

        while (options.Count < 3 && safety < 20)
        {
            UpgradeOption option = GenerateSingleOption(loadout, options);

            if (option != null)
                options.Add(option);

            safety++;
        }

        if (options.Count == 0)
        {
            options.Add(CreateFallback(StatType.Damage, 0.1f));
            options.Add(CreateFallback(StatType.AttackSpeed, 0.1f));
            options.Add(CreateFallback(StatType.MovementSpeed, 0.1f));
            return options;
        }

        while (options.Count < 3)
        {
            UpgradeOption fallback = CreateFallback(StatType.Damage, 0.05f);

            if (!ContainsSimilarOption(options, fallback))
                options.Add(fallback);
            else
                break;
        }

        return options;
    }

    private UpgradeOption CreateFallback(StatType stat, float value)
    {
        UpgradeOption option = new UpgradeOption();

        option.Effects.Add(new UpgradeEffect
        {
            EffectType = UpgradeEffectType.GlobalModifier,
            TargetStat = stat,
            ModifierType = ModifierType.Percent,
            Value = value
        });

        return option;
    }

    private bool ContainsSimilarOption(List<UpgradeOption> options, UpgradeOption newOption)
    {
        foreach (UpgradeOption option in options)
        {
            foreach (UpgradeEffect effect in option.Effects)
            {
                foreach (UpgradeEffect newEffect in newOption.Effects)
                {
                    if (effect.TargetStat == newEffect.TargetStat &&
                        effect.ModifierType == newEffect.ModifierType)
                        return true;
                }
            }
        }

        return false;
    }

    private UpgradeOption GenerateSingleOption(
        PlayerSkillLoadout loadout,
        List<UpgradeOption> existingOptions)
    {
        bool hasFreeSlot = loadout.HasFreeSlot();

        if (hasFreeSlot)
            return GenerateWeightedOption(loadout, existingOptions, true);

        if (Random.value <= newSkillChanceWhenFull)
            return GenerateNewSkillOption(loadout, existingOptions);

        return GenerateWeightedOption(loadout, existingOptions, false);
    }

    private UpgradeOption GenerateWeightedOption(
        PlayerSkillLoadout loadout,
        List<UpgradeOption> existingOptions,
        bool allowNewSkill)
    {
        float totalWeight = modifierWeight + globalModifierWeight;

        if (allowNewSkill)
            totalWeight += newSkillWeight;

        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        if (allowNewSkill)
        {
            cumulative += newSkillWeight;

            if (roll <= cumulative)
            {
                UpgradeOption option = GenerateNewSkillOption(loadout, existingOptions);
                if (option != null)
                    return option;
            }
        }

        cumulative += modifierWeight;

        if (roll <= cumulative)
        {
            UpgradeOption option = GenerateModifierOption(loadout, existingOptions);
            if (option != null)
                return option;
        }

        return GenerateGlobalOption(existingOptions);
    }

    private UpgradeOption GenerateNewSkillOption(
        PlayerSkillLoadout loadout,
        List<UpgradeOption> existingOptions)
    {
        availableSkills.Clear();

        foreach (SkillDefinition skill in database.AllSkills)
        {
            if (skill == null)
                continue;

            if (SkillAlreadyOwned(loadout, skill))
                continue;

            if (SkillAlreadyInOptions(existingOptions, skill))
                continue;

            availableSkills.Add(skill);
        }

        if (availableSkills.Count == 0)
            return null;

        SkillDefinition selected = GetWeightedByRarity(
            availableSkills,
            s => s.Rarity
        );

        UpgradeOption option = new UpgradeOption();
        option.SkillDefinition = selected;

        option.Effects.Add(new UpgradeEffect
        {
            EffectType = UpgradeEffectType.AddNewSkill
        });

        return option;
    }

    private UpgradeOption GenerateModifierOption(
        PlayerSkillLoadout loadout,
        List<UpgradeOption> existingOptions)
    {
        ownedSkills.Clear();

        RuntimeSkill[] allSkills = loadout.GetAllSkills();

        for (int i = 0; i < allSkills.Length; i++)
        {
            if (allSkills[i] != null)
                ownedSkills.Add(allSkills[i]);
        }

        if (ownedSkills.Count == 0)
            return null;

        RuntimeSkill randomSkill = ownedSkills[Random.Range(0, ownedSkills.Count)];

        List<SkillModifierDefinition> possible =
            randomSkill.Definition.PossibleModifiers;

        if (possible == null || possible.Count == 0)
            return null;

        filteredModifiers.Clear();

        for (int i = 0; i < possible.Count; i++)
        {
            SkillModifierDefinition modifier = possible[i];

            if (modifier == null)
                continue;

            if (!ModifierAlreadyInOptions(existingOptions, modifier))
                filteredModifiers.Add(modifier);
        }

        if (filteredModifiers.Count == 0)
            return null;

        SkillModifierDefinition selected = GetWeightedByRarity(
            filteredModifiers,
            m => m.Rarity
        );

        int slotIndex = System.Array.IndexOf(allSkills, randomSkill);

        UpgradeOption option = new UpgradeOption();

        option.Effects.Add(new UpgradeEffect
        {
            EffectType = UpgradeEffectType.SkillModifier,
            TargetSlotIndex = slotIndex,
            SkillModifier = selected
        });

        return option;
    }

    private UpgradeOption GenerateGlobalOption(List<UpgradeOption> existingOptions)
    {
        if (globalUpgrades == null || globalUpgrades.Count == 0)
            return null;

        filteredGlobals.Clear();

        for (int i = 0; i < globalUpgrades.Count; i++)
        {
            GlobalUpgradeDefinition global = globalUpgrades[i];

            if (global == null)
                continue;

            if (!GlobalAlreadyInOptions(existingOptions, global))
                filteredGlobals.Add(global);
        }

        if (filteredGlobals.Count == 0)
            return null;

        GlobalUpgradeDefinition selected = GetWeightedByRarity(
            filteredGlobals,
            g => g.Rarity
        );

        UpgradeOption option = new UpgradeOption();

        option.Effects.Add(new UpgradeEffect
        {
            EffectType = UpgradeEffectType.GlobalModifier,
            GlobalDefinition = selected,
            TargetStat = selected.TargetStat,
            ModifierType = selected.ModifierType,
            Value = selected.Value
        });

        return option;
    }

    private T GetWeightedByRarity<T>(
        List<T> items,
        System.Func<T, SkillRarity> raritySelector)
    {
        float totalWeight = 0f;

        for (int i = 0; i < items.Count; i++)
            totalWeight += GetRarityWeight(raritySelector(items[i]));

        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        for (int i = 0; i < items.Count; i++)
        {
            cumulative += GetRarityWeight(raritySelector(items[i]));

            if (roll <= cumulative)
                return items[i];
        }

        return items[0];
    }

    private float GetRarityWeight(SkillRarity rarity)
    {
        switch (rarity)
        {
            case SkillRarity.Common: return 60f;
            case SkillRarity.Rare: return 25f;
            case SkillRarity.Epic: return 10f;
            case SkillRarity.Legendary: return 5f;
            default: return 1f;
        }
    }

    private bool SkillAlreadyOwned(PlayerSkillLoadout loadout, SkillDefinition skill)
    {
        RuntimeSkill[] skills = loadout.GetAllSkills();

        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i] != null && skills[i].Definition == skill)
                return true;
        }

        return false;
    }

    private bool SkillAlreadyInOptions(List<UpgradeOption> options, SkillDefinition skill)
    {
        foreach (UpgradeOption option in options)
        {
            if (option.SkillDefinition == skill)
                return true;
        }

        return false;
    }

    private bool ModifierAlreadyInOptions(
        List<UpgradeOption> options,
        SkillModifierDefinition modifier)
    {
        foreach (UpgradeOption option in options)
        {
            foreach (UpgradeEffect effect in option.Effects)
            {
                if (effect.SkillModifier == modifier)
                    return true;
            }
        }

        return false;
    }

    private bool GlobalAlreadyInOptions(
        List<UpgradeOption> options,
        GlobalUpgradeDefinition global)
    {
        foreach (UpgradeOption option in options)
        {
            foreach (UpgradeEffect effect in option.Effects)
            {
                if (effect.GlobalDefinition == global)
                    return true;
            }
        }

        return false;
    }
}