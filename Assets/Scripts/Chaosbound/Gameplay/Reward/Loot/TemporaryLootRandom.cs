using UnityEngine;

public sealed class TemporaryLootRandom : ILootRandom
{
    public float Value()
    {
        return Random.value;
    }
}