using UnityEngine;
using UnityEngine.UI;

public class HUDXPBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private float fillSpeed = 5f;

    private PlayerExperienceSystem experience;
    private float targetFill;

    private void Update()
    {
        if (fillImage == null)
            return;

        fillImage.fillAmount = Mathf.Lerp(
            fillImage.fillAmount,
            targetFill,
            Time.deltaTime * fillSpeed);
    }

    public void Bind(
        PlayerExperienceSystem xpSystem)
    {
        if (experience != null)
            experience.OnXPChanged -= UpdateXP;

        experience = xpSystem;

        if (experience == null)
            return;

        experience.OnXPChanged += UpdateXP;

        UpdateXP(
            experience.CurrentXP,
            experience.RequiredXP);
    }

    private void OnDestroy()
    {
        if (experience != null)
            experience.OnXPChanged -= UpdateXP;
    }

    private void UpdateXP(
        float current,
        float max)
    {
        if (max <= 0f)
        {
            targetFill = 0f;
            return;
        }

        targetFill =
            Mathf.Clamp01(current / max);
    }
}