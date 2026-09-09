using UnityEngine;

/// <summary>
/// Restores the prefab-authored local rotation whenever the object is enabled.
/// Useful for pooled presentation objects whose spawn placement supplies a
/// runtime rotation that should not affect their authored visual orientation.
/// </summary>
public sealed class PreservePrefabRotation : MonoBehaviour
{
    private Quaternion authoredLocalRotation;

    private void Awake()
    {
        authoredLocalRotation = transform.localRotation;
    }

    private void OnEnable()
    {
        transform.localRotation = authoredLocalRotation;
    }
}
