using UnityEngine;

public class StormfallImpactParts : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject beam;

    public GameObject Beam => beam;

    private void OnValidate()
    {
        if (beam == null)
            Debug.LogWarning($"{name}: Beam reference is missing.", this);
    }
}