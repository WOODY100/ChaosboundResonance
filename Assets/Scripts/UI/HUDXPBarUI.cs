using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDXPBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private float fillSpeed = 5f;

    private PlayerExperienceSystem experience;
    private float targetFill;

    void Update()
    {
        fillImage.fillAmount = Mathf.Lerp(
            fillImage.fillAmount,
            targetFill,
            Time.deltaTime * fillSpeed
        );
    }

    public void Bind(PlayerExperienceSystem xpSystem)
    {
        experience = xpSystem;

        experience.OnXPChanged += UpdateXP;

        UpdateXP(experience.CurrentXP, experience.RequiredXP);
    }

    private void OnDestroy()
    {
        if (experience != null)
            experience.OnXPChanged -= UpdateXP;
    }

    private void UpdateXP(float current, float max)
    {
        targetFill = current / max;

        if (xpText != null)
            xpText.text = $"{Mathf.FloorToInt(current)} / {Mathf.FloorToInt(max)}";
    }
}