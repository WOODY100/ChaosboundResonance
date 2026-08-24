using System;
using UnityEngine;

[Serializable]
public sealed class EnemyRewardEntryDefinition
{
    [Header("Reward")]

    [SerializeField]
    private EnemyRewardType m_Type;

    [SerializeField]
    private string m_ContentId;

    [Header("Resolution")]

    [SerializeField]
    [Range(0f, 1f)]
    private float m_Chance = 1f;

    [SerializeField]
    private int m_Amount = 1;

    public EnemyRewardType Type =>
        m_Type;

    public string ContentId =>
        m_ContentId;

    public float Chance =>
        m_Chance;

    public int Amount =>
        m_Amount;

    public bool IsGuaranteed =>
        m_Chance >= 1f;

    private void OnValidate()
    {
        m_Chance =
            Mathf.Clamp01(
                m_Chance);

        m_Amount =
            Mathf.Max(
                0,
                m_Amount);
    }
}