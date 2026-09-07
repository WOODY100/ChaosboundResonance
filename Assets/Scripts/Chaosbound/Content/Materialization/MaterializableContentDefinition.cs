using UnityEngine;

[CreateAssetMenu(
    menuName = "Chaosbound/Content/Materializable Content Definition")]
public sealed class MaterializableContentDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField]
    private string contentId;

    [Header("Physical Representation")]
    [SerializeField]
    private GameObject prefab;

    public string ContentId => contentId;

    public GameObject Prefab => prefab;

    private void OnValidate()
    {
        contentId = contentId != null
            ? contentId.Trim()
            : string.Empty;
    }
}