using UnityEngine;

public struct DamageData
{
    public float amount;
    public GameObject source;
    public bool isCrit;
    public string skillDefinitionId;
    public string executionId;
    public string analyticsSourceCategory;
    public string skillId;
    public string skillDisplayName;

    public DamageData(float amount, bool isCrit = false)
        : this(amount, isCrit, null, null, null, null, null)
    {
    }

    public DamageData(
        float amount,
        bool isCrit,
        string skillDefinitionId,
        string executionId)
        : this(amount, isCrit, skillDefinitionId, executionId, null, null, null)
    {
    }

    public DamageData(
        float amount,
        bool isCrit,
        string skillDefinitionId,
        string executionId,
        string analyticsSourceCategory)
        : this(amount, isCrit, skillDefinitionId, executionId,
            analyticsSourceCategory, null, null)
    {
    }

    public DamageData(
        float amount,
        bool isCrit,
        string skillDefinitionId,
        string executionId,
        string analyticsSourceCategory,
        string skillId,
        string skillDisplayName)
    {
        this.amount = amount;
        this.source = null;
        this.isCrit = isCrit;
        this.skillDefinitionId = skillDefinitionId;
        this.executionId = executionId;
        this.analyticsSourceCategory = analyticsSourceCategory;
        this.skillId = skillId;
        this.skillDisplayName = skillDisplayName;
    }
}
