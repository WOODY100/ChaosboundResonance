using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    menuName = "Chaosbound/Enemies/Rewards/Enemy Reward Definition")]
public sealed class EnemyRewardDefinition :
    ScriptableObject
{
    [SerializeField]
    private List<EnemyRewardEntryDefinition> m_Rewards =
        new();

    public IReadOnlyList<EnemyRewardEntryDefinition> Rewards =>
        m_Rewards;

    private void OnValidate()
    {
        m_Rewards ??=
            new List<EnemyRewardEntryDefinition>();
    }
}