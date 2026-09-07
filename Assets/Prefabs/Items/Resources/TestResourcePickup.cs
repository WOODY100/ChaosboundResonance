using UnityEngine;

public sealed class TestResourcePickup : MonoBehaviour, IResourcePickup
{
    private int amount;

    public int Amount => amount;

    public void Initialize(int amount)
    {
        if (amount <= 0)
            throw new System.ArgumentOutOfRangeException(nameof(amount));

        this.amount = amount;

        Debug.Log(
            $"TestResourcePickup initialized with amount: {amount}.",
            this);
    }
}