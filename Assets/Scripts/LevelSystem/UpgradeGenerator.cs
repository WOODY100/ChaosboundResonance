using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

        return options;
    }

    private UpgradeOption GenerateSingleOption(
        PlayerSkillLoadout loadout,
        List<UpgradeOption> existingOptions)
    {
        bool hasFreeSlot = loadout.HasFreeSlot();

        if (hasFreeSlot)
        {
            return GenerateWeightedOption(loadout, existingOptions, true);
        }
        else
        {
            if (Random.value <= newSkillChanceWhenFull)
                return GenerateNewSkillOption(loadout, existingOptions);

            return GenerateWeightedOption(loadout, existingOptions, false);
        }
    }

    private UpgradeOption GenerateWeightedOption(
        PlayerSkillLoadout loadout,
        List<UpgradeOption> existingOptions,
        bool allowNewSkill)
    {
        List<(System.Func<UpgradeOption> generator, float weight)> pool =
            new List<(System.Func<UpgradeOption>, float)>();

        if (allowNewSkill)
        {
            pool.Add((() => GenerateNewSkillOption(loadout, existingOptions), newSkillWeight));
            pool.Add((() => GenerateModifierOption(loadout, existingOptions), modifierWeight));
            pool.Add((() => GenerateGlobalOption(existingOptions), globalModifierWeight));

        }

        float totalWeight = pool.Sum(p => p.weight);
        float roll = Random.value * totalWeight;

        float cumulative = 0f;

        foreach (var entry in pool)
        {
            cumulative += entry.weight;
            if (roll <= cumulative)
                return entry.generator();
        }

        return null;
    }

    // ===========================
    // NEW SKILL
    // ===========================

    private UpgradeOption GenerateNewSkillOption(
        PlayerSkillLoadout loadout,
        List<UpgradeOption> existingOptions)
    {
        List<SkillDefinition> available =
            database.AllSkills
            .Where(s =>
                !SkillAlreadyOwned(loadout, s) &&
                !SkillAlreadyInOptions(existingOptions, s))
            .ToList();

        if (available.Count == 0)
            return null;

        SkillDefinition selected =
            GetWeightedByRarity(available, s => s.Rarity);

        UpgradeOption option = new UpgradeOption();
        option.SkillDefinition = selected;

        option.Effects.Add(new UpgradeEffect
        {
            EffectType = UpgradeEffectType.AddNewSkill
        });

        return option;
    }

    // ===========================
    // MODIFIER
    // ===========================

    private UpgradeOption GenerateModifierOption(
        PlayerSkillLoadout loadout,
        List<UpgradeOption> existingOptions)
    {
        List<RuntimeSkill> owned =
            loadout.GetAllSkills()
            .Where(s => s != null)
            .ToList();

        if (owned.Count == 0)
            return null;

        RuntimeSkill randomSkill =
            owned[Random.Range(0, owned.Count)];

        List<SkillModifierDefinition> possible =
            randomSkill.Definition.PossibleModifiers;

        if (possible == null || possible.Count == 0)
            return null;

        List<SkillModifierDefinition> filtered =
            possible.Where(m =>
                !ModifierAlreadyInOptions(existingOptions, m))
            .ToList();

        if (filtered.Count == 0)
            return null;

        SkillModifierDefinition modifier =
            GetWeightedByRarity(filtered, m => m.Rarity);

        int slotIndex = System.Array.IndexOf(
            loadout.GetAllSkills(), randomSkill);

        UpgradeOption option = new UpgradeOption();

        option.Effects.Add(new UpgradeEffect
        {
            EffectType = UpgradeEffectType.SkillModifier,
            TargetSlotIndex = slotIndex,
            SkillModifier = modifier
        });

        return option;
    }

    // ===========================
    // UTILITIES
    // ===========================

    private T GetWeightedByRarity<T>(
        List<T> items,
        System.Func<T, SkillRarity> raritySelector)
    {
        float totalWeight = 0f;
        Dictionary<T, float> weights = new Dictionary<T, float>();

        foreach (var item in items)
        {
            float weight = GetRarityWeight(raritySelector(item));
            weights[item] = weight;
            totalWeight += weight;
        }

        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var pair in weights)
        {
            cumulative += pair.Value;
            if (roll <= cumulative)
                return pair.Key;
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
        }

        return 1f;
    }

    private bool SkillAlreadyOwned(
        PlayerSkillLoadout loadout,
        SkillDefinition skill)
    {
        foreach (var s in loadout.GetAllSkills())
        {
            if (s != null && s.Definition == skill)
                return true;
        }

        return false;
    }

    private bool SkillAlreadyInOptions(
        List<UpgradeOption> options,
        SkillDefinition skill)
    {
        foreach (var option in options)
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
        foreach (var option in options)
        {
            foreach (var effect in option.Effects)
            {
                if (effect.SkillModifier == modifier)
                    return true;
            }
        }

        return false;
    }

    private UpgradeOption GenerateGlobalOption(
    List<UpgradeOption> existingOptions)
    {
        if (globalUpgrades == null || globalUpgrades.Count == 0)
            return null;

        List<GlobalUpgradeDefinition> filtered =
            globalUpgrades
            .Where(g => !GlobalAlreadyInOptions(existingOptions, g))
            .ToList();

        if (filtered.Count == 0)
            return null;

        GlobalUpgradeDefinition selected =
            GetWeightedByRarity(filtered, g => g.Rarity);

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

    private bool GlobalAlreadyInOptions(
    List<UpgradeOption> options,
    GlobalUpgradeDefinition global)
    {
        foreach (var option in options)
        {
            foreach (var effect in option.Effects)
            {
                if (effect.GlobalDefinition == global)
                    return true;
            }
        }

        return false;
    }
}