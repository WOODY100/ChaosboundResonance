using UnityEngine;

public interface IProjectile
{
    void Initialize(
        RuntimeSkill skill,
        Vector3 direction,
        PlayerStats ownerStats
    );
}