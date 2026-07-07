using System.Collections;
using TMPro;
using UnityEngine;

public class HUDLevelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;

    private PlayerExperienceSystem experience;

    public void Bind(PlayerExperienceSystem xpSystem)
    {
        experience = xpSystem;

        experience.OnLevelUp += UpdateLevelText;
        UpdateLevelText(experience.CurrentLevel);
    }

    private void OnDestroy()
    {
        if (experience != null)
            experience.OnLevelUp -= UpdateLevelText;
    }

    private void UpdateLevelText(int level)
    {
        if (levelText == null)
            return;

        levelText.SetText("{0}", level);
        StartCoroutine(LevelPulse());
    }

    private IEnumerator LevelPulse()
    {
        Vector3 original = levelText.transform.localScale;
        levelText.transform.localScale = original * 1.2f;
        yield return new WaitForSeconds(0.1f);
        levelText.transform.localScale = original;
    }
}