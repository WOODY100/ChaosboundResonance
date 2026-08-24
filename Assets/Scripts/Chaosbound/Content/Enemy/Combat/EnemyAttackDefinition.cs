using System;
using UnityEngine;

[Serializable]
public sealed class EnemyAttackDefinition
{
    [SerializeField]
    private DamageType m_DamageType = DamageType.Physical;

    [Header("Hit Area")]

    [SerializeField]
    private float m_Range = 2.2f;

    [SerializeField]
    [Range(0f, 360f)]
    private float m_Angle = 90f;

    [Header("Timing")]

    [SerializeField]
    private float m_Cooldown = 1f;

    [SerializeField]
    private AnimationClip m_AnimationClip;

    [SerializeField]
    [Range(0f, 1f)]
    private float m_ImpactNormalizedTime = 0.45f;

    [Header("Ability")]

    [SerializeField]
    private EnemyAbilityDefinition m_Ability;

    public DamageType DamageType =>
        m_DamageType;

    public float Range =>
        m_Range;

    public float Angle =>
        m_Angle;

    public float Cooldown =>
        m_Cooldown;

    public AnimationClip AnimationClip =>
        m_AnimationClip;

    public float ImpactNormalizedTime =>
        m_ImpactNormalizedTime;

    public EnemyAbilityDefinition Ability =>
        m_Ability;

    public float Duration
    {
        get
        {
            if (m_AnimationClip == null)
                return 0f;

            return m_AnimationClip.length;
        }
    }

    public float ImpactTime
    {
        get
        {
            if (m_AnimationClip == null)
                return 0f;

            return
                m_AnimationClip.length *
                m_ImpactNormalizedTime;
        }
    }

    private void OnValidate()
    {
        m_Range =
            Mathf.Max(
                0f,
                m_Range);

        m_Cooldown =
            Mathf.Max(
                0f,
                m_Cooldown);

        m_ImpactNormalizedTime =
            Mathf.Clamp01(
                m_ImpactNormalizedTime);
    }
}