using UnityEngine;

public struct DamageData
{
    public float amount;
    public GameObject source;
    public bool isCrit;

    public DamageData(
        float amount,
        bool isCrit = false)
    {
        this.amount = amount;
        this.source = null;
        this.isCrit = isCrit;
    }
}