using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SkillSlotUI : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private GameObject emptyIcon;
    [SerializeField] private RectTransform runeTransform;
    [SerializeField] private Image runeImage;
    [SerializeField] private Outline runeOutline;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private GameObject frameNormal;
    [SerializeField] private GameObject frameReplace;
    [SerializeField] private GameObject evolutionsContainer;
    [SerializeField] private GameObject[] evolutionSlots; // Evolution_0,1,2
    [SerializeField] private Image[] evolutionIcons;      // solo los Icon
    [Header("Rune FX")]
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float breatheSpeed = 1.5f;
    [SerializeField] private float minAlpha = 0.55f;
    [SerializeField] private float maxAlpha = 0.8f;
    [Header("Hover & Swap FX")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float hoverSpeed = 10f;
    [SerializeField] private float swapDuration = 0.2f;
    [SerializeField] private Image flashImage; // opcional overlay blanco
    [SerializeField] private RectTransform iconTransform;
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float pulseDuration = 0.15f;

    private Vector3 originalScale;
    private bool isHovered;
    private int slotIndex;
    private bool wasOnCooldown;
    private bool previousCooldownState;
    private float previousCooldownValue;
    private bool isReplaceMode;
    private RuntimeSkill currentSkill;
    private LevelUpManager levelUpManager;
    private Coroutine summonRoutine;

    void Start()
    {
        originalScale = transform.localScale;
        levelUpManager = FindFirstObjectByType<LevelUpManager>();
        DisableReplaceMode();

        // 🔥 Ocultar evoluciones al iniciar
        if (evolutionsContainer != null)
            evolutionsContainer.SetActive(false);

        if (emptyIcon != null)
            emptyIcon.SetActive(true);

        if (flashImage != null)
        {
            flashImage.color = new Color(1f, 1f, 1f, 0f);
            flashImage.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (currentSkill != null)
        {
            currentSkill.OnCooldownFinished -= HandleCooldownFinished;
            currentSkill.OnEvolutionApplied -= HandleEvolutionApplied;
        }

    }

    private void Update()
    {
        // 🔥 SLOT VACÍO → animar runa
        if (currentSkill == null)
        {
            UpdateRuneFX();
        }
        else if (cooldownOverlay != null)
        {
            // 🔥 Actualizar radial cooldown
            if (currentSkill.IsOnCooldown)
            {
                cooldownOverlay.enabled = true;

                float percent =
                    currentSkill.CurrentCooldown /
                    currentSkill.CooldownDuration;

                cooldownOverlay.fillAmount = percent;
            }
            else
            {
                cooldownOverlay.enabled = false;
                cooldownOverlay.fillAmount = 0f;
            }
        }

        // 🔥 Hover scale (esto siempre debe correr)
        Vector3 targetScale = isHovered
            ? originalScale * hoverScale
            : originalScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.unscaledDeltaTime * hoverSpeed);
    }

    public void SetIndex(int index)
    {
        slotIndex = index;
    }

    public void SetSkill(RuntimeSkill skill)
    {
        // 🔥 Desuscribirse del anterior
        if (currentSkill != null)
        {
            currentSkill.OnCooldownFinished -= HandleCooldownFinished;
            currentSkill.OnEvolutionApplied -= HandleEvolutionApplied;
        }

        currentSkill = skill;

        // ===============================
        // SLOT VACÍO
        // ===============================
        if (currentSkill == null)
        {
            iconImage.enabled = false;

            if (emptyIcon != null)
                emptyIcon.SetActive(true);

            // Reset visual rune
            if (runeTransform != null)
                runeTransform.localRotation = Quaternion.identity;

            if (runeImage != null)
                runeImage.color = Color.white;

            RefreshEvolutionsUI();
            return;
        }

        // ===============================
        // SLOT CON SKILL
        // ===============================
        if (summonRoutine != null)
            StopCoroutine(summonRoutine);

        summonRoutine = StartCoroutine(
            SummonSkillRoutine(currentSkill.Definition.Icon)
        );

        // Suscribirse eventos
        currentSkill.OnCooldownFinished += HandleCooldownFinished;
        currentSkill.OnEvolutionApplied += HandleEvolutionApplied;

        DisableReplaceMode();
        RefreshEvolutionsUI();
    }

    public void EnableReplaceMode()
    {
        if (currentSkill == null)
            return;

        frameNormal.SetActive(false);
        frameReplace.SetActive(true);
    }

    public void DisableReplaceMode()
    {
        frameNormal.SetActive(true);
        frameReplace.SetActive(false);
    }

    public void PlaySwapAnimation()
    {
        StartCoroutine(SwapRoutine());
    }

    private IEnumerator SwapRoutine()
    {
        float timer = 0f;

        Vector3 startScale = originalScale;
        Vector3 shrinkScale = originalScale * 0.8f;

        // Shrink
        while (timer < swapDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / swapDuration;
            transform.localScale = Vector3.Lerp(startScale, shrinkScale, t);
            yield return null;
        }

        // Flash (si existe)
        if (flashImage != null)
        {
            flashImage.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.05f);
            flashImage.gameObject.SetActive(false);
        }

        timer = 0f;

        // Bounce back
        while (timer < swapDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / swapDuration;
            transform.localScale = Vector3.Lerp(shrinkScale, startScale, t);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!frameReplace.activeSelf)
            return;

        PlaySwapAnimation(); // 🔥 primero animación

        levelUpManager.ReplaceSkillAt(slotIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!frameReplace.activeSelf) return;
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public void PlayFlash()
    {
        if (flashImage == null) return;

        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        flashImage.gameObject.SetActive(true);

        float duration = 0.12f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float alpha = Mathf.Lerp(1f, 0f, timer / duration);

            flashImage.color =
                new Color(1f, 1f, 1f, alpha);

            yield return null;
        }

        flashImage.color = new Color(1f, 1f, 1f, 0f);
        flashImage.gameObject.SetActive(false);
    }

    public void PlayPulse()
    {
        if (iconTransform == null) return;

        StopCoroutine(nameof(PulseRoutine));
        StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        Vector3 original = iconTransform.localScale;
        Vector3 target = original * pulseScale;

        float timer = 0f;

        // Expand
        while (timer < pulseDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / pulseDuration;

            iconTransform.localScale =
                Vector3.Lerp(original, target, t);

            yield return null;
        }

        timer = 0f;

        // Return
        while (timer < pulseDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / pulseDuration;

            iconTransform.localScale =
                Vector3.Lerp(target, original, t);

            yield return null;
        }

        iconTransform.localScale = original;
    }

    private void HandleCooldownFinished(RuntimeSkill skill)
    {
        PlayFlash();
        PlayPulse();
    }

    private void RefreshEvolutionsUI()
    {
        if (evolutionsContainer == null)
            return;

        if (currentSkill == null)
        {
            evolutionsContainer.SetActive(false);
            return;
        }

        var evolutions = currentSkill.Evolutions;

        if (evolutions == null || evolutions.Count == 0)
        {
            evolutionsContainer.SetActive(false);
            return;
        }

        evolutionsContainer.SetActive(true);

        for (int i = 0; i < evolutionSlots.Length; i++)
        {
            if (i < evolutions.Count)
            {
                evolutionSlots[i].SetActive(true);
                evolutionIcons[i].sprite = evolutions[i].Icon;
            }
            else
            {
                evolutionSlots[i].SetActive(false);
            }
        }
    }

    private void HandleEvolutionApplied(RuntimeSkill skill)
    {
        RefreshEvolutionsUI();
    }

    private void UpdateRuneFX()
    {
        if (runeTransform != null)
        {
            runeTransform.Rotate(0f, 0f,
                rotationSpeed * Time.unscaledDeltaTime);
        }

        if (runeImage != null)
        {
            float alpha =
                Mathf.Lerp(minAlpha, maxAlpha,
                (Mathf.Sin(Time.unscaledTime * breatheSpeed) + 1f) * 0.5f);

            runeImage.color =
                new Color(1f, 1f, 1f, alpha);
        }

        if (runeOutline != null)
        {
            float t =
                (Mathf.Sin(Time.unscaledTime * 1.2f) + 1f) * 0.5f;

            runeOutline.effectColor =
                Color.Lerp(
                    new Color(0.25f, 0.45f, 1f, 0.5f),
                    new Color(0.6f, 0.2f, 1f, 0.8f),
                    t);
        }
    }

    private IEnumerator SummonSkillRoutine(Sprite newIcon)
    {
        // 🔥 Asegurarse que el icono esté oculto
        iconImage.enabled = false;

        // Activar runa si no está activa
        if (emptyIcon != null)
            emptyIcon.SetActive(true);

        // 1️⃣ Rotación rápida
        float duration = 0.2f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float speedMultiplier = 8f; // qué tan rápido gira
            runeTransform.Rotate(0f, 0f,
                rotationSpeed * speedMultiplier * Time.unscaledDeltaTime);

            yield return null;
        }

        // 2️⃣ Flash blanco fuerte
        if (flashImage != null)
        {
            flashImage.gameObject.SetActive(true);
            flashImage.color = new Color(1f, 1f, 1f, 1f);

            yield return new WaitForSecondsRealtime(0.05f);

            flashImage.color = new Color(1f, 1f, 1f, 0f);
            flashImage.gameObject.SetActive(false);
        }

        // 3️⃣ Apagar runa
        if (emptyIcon != null)
            emptyIcon.SetActive(false);

        // Reset visual
        if (runeTransform != null)
            runeTransform.localRotation = Quaternion.identity;

        if (runeImage != null)
            runeImage.color = Color.white;

        // 4️⃣ Mostrar icono
        iconImage.enabled = true;
        iconImage.sprite = newIcon;
    }
}