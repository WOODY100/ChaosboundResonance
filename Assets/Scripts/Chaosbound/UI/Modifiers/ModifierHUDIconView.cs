using Chaosbound.Gameplay.ExpeditionRuntime.Modifiers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chaosbound.UI.Modifiers
{
    public sealed class ModifierHUDIconView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image iconImage;
        [SerializeField] private Image durationFillImage;
        [SerializeField] private TMP_Text stackCountText;

        public string ApplicationId { get; private set; }

        public void Bind(
            ExpeditionModifier modifier,
            Sprite icon,
            TimeSpan elapsedTime)
        {
            if (modifier == null)
                throw new ArgumentNullException(nameof(modifier));

            ApplicationId = modifier.ApplicationId;

            if (iconImage != null)
                iconImage.sprite = icon;

            Refresh(modifier, elapsedTime);
        }

        public void Refresh(
            ExpeditionModifier modifier,
            TimeSpan elapsedTime)
        {
            if (modifier == null)
                return;

            if (stackCountText != null)
                stackCountText.text = modifier.StackCount.ToString();

            if (durationFillImage != null)
            {
                durationFillImage.fillAmount =
                    CalculateFillAmount(modifier, elapsedTime);
            }
        }

        /// <summary>
        /// Projects the domain-owned remaining duration into a normalized HUD fill.
        /// The HUD never changes or owns the modifier's lifetime.
        /// </summary>
        public static float CalculateFillAmount(
            ExpeditionModifier modifier,
            TimeSpan elapsedTime)
        {
            if (modifier == null || !modifier.IsTimed)
                return 1f;

            if (modifier.TotalGrantedDuration <= TimeSpan.Zero)
                return 0f;

            TimeSpan elapsedSinceUpdate =
                elapsedTime - modifier.LastUpdatedAt;

            if (elapsedSinceUpdate < TimeSpan.Zero)
                elapsedSinceUpdate = TimeSpan.Zero;

            TimeSpan remaining =
                modifier.RemainingDuration - elapsedSinceUpdate;

            if (remaining < TimeSpan.Zero)
                remaining = TimeSpan.Zero;

            float fill =
                (float)(remaining.TotalSeconds /
                        modifier.TotalGrantedDuration.TotalSeconds);

            return Mathf.Clamp01(fill);
        }
    }
}
