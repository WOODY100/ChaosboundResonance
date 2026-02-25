using UnityEngine;

public struct DamageData
{
    public float amount;
    public DamageType type;
    public GameObject source;
    public bool isCrit;

    public DamageData(float amount, DamageType type, bool isCrit = false)
    {
        this.amount = amount;
        this.type = type;
        this.source = null;
        this.isCrit = isCrit;
    }
}