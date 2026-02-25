using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UpgradeGenerator : MonoBehaviour
{
    [SerializeField] private SkillDatabase database;

    [Header("Weights")]
    [SerializeField] private float newSkillWeight = 1f;
    [SerializeField] private float modifierWeight = 2f;

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
            {
                return GenerateNewSkillOption(loadout, existingOptions);
            }

            return GenerateWeightedOption(loadout, existingOptions, false);
        }
    }

    private UpgradeOption GenerateWeightedOption(
        PlayerSkillLoadout loadout,
        List<UpgradeOption> existingOptions,
        bool allowNewSkill)
    {
        List<(UpgradeType type, float weight)> pool =
            new List<(UpgradeType, float)>();

        if (allowNewSkill)
            pool.Add((UpgradeType.NewSkill, newSkillWeight));

        pool.Add((UpgradeType.SkillModifier, modifierWeight));

        float totalWeight = pool.Sum(p => p.weight);
        float roll = Random.value * totalWeight;

        float cumulative = 0f;

        foreach (var entry in pool)
        {
            cumulative += entry.weight;

            if (roll <= cumulative)
            {
                switch (entry.type)
                {
                    case UpgradeType.NewSkill:
                        return GenerateNewSkillOption(loadout, existingOptions);

                    case UpgradeType.SkillModifier:
                        return GenerateModifierOption(loadout, existingOptions);
                }
            }
        }

        return null;
    }

    // ===========================
    // OPTION TYPES
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
            GetWeightedSkillByRarity(available);

        return new UpgradeOption
        {
            Type = UpgradeType.NewSkill,
            SkillDefinition = selected
        };
    }

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

        // Filtrar modifiers ya presentes en opciones actuales
        List<SkillModifierDefinition> filtered =
            possible.Where(m =>
                !ModifierAlreadyInOptions(existingOptions, m))
            .ToList();

        if (filtered.Count == 0)
            return null;

        SkillModifierDefinition modifier =
            GetWeightedByRarity(
                filtered,
                m => m.Rarity
            );

        int slotIndex = System.Array.IndexOf(
            loadout.GetAllSkills(), randomSkill);

        return new UpgradeOption
        {
            Type = UpgradeType.SkillModifier,
            ModifierDefinition = modifier,
            TargetSlotIndex = slotIndex
        };
    }

    private bool SkillAlreadyInOptions(
    List<UpgradeOption> options,
    SkillDefinition skill)
    {
        foreach (var option in options)
        {
            if (option.Type == UpgradeType.NewSkill &&
                option.SkillDefinition == skill)
                return true;
        }

        return false;
    }

    // ===========================
    // UTILITIES
    // ===========================

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

    private SkillDefinition GetWeightedSkillByRarity(
    List<SkillDefinition> skills)
    {
        return GetWeightedByRarity(
            skills,
            s => s.Rarity
        );
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

    private bool ModifierAlreadyInOptions(
    List<UpgradeOption> options,
    SkillModifierDefinition modifier)
    {
        foreach (var option in options)
        {
            if (option.Type == UpgradeType.SkillModifier &&
                option.ModifierDefinition == modifier)
                return true;
        }

        return false;
    }
}