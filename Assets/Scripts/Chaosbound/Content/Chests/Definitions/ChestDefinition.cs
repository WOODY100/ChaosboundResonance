using UnityEngine;

[CreateAssetMenu(
    fileName = "ChestDefinition",
    menuName = "Chaosbound/Chests/Chest Definition")]
public sealed class ChestDefinition : ScriptableObject
{
    [Header("Identity")]

    [SerializeField]
    private string chestId;

    [Header("Rewards")]

    [SerializeField]
    private LootDefinition lootDefinition;

    public string ChestId => chestId;

    public LootDefinition LootDefinition =>
        lootDefinition;
}