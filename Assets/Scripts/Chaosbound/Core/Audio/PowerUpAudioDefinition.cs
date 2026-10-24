using UnityEngine;

[CreateAssetMenu(menuName = "Power Ups/Power Up Audio Definition")]
public sealed class PowerUpAudioDefinition : ScriptableObject
{
    [Header("Pickup")]
    [SerializeField]
    private AudioClip pickupClip;

    [SerializeField, Range(0f, 1f)]
    private float pickupVolume = 1f;

    public AudioClip PickupClip => pickupClip;

    public float PickupVolume => pickupVolume;

    private void OnValidate()
    {
        pickupVolume = Mathf.Clamp01(pickupVolume);
    }
}