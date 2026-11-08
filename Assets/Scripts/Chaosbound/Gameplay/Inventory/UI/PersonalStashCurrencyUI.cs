using Chaosbound.Core.Composition;
using TMPro;
using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class PersonalStashCurrencyUI : MonoBehaviour
    {
        [Header("Currency")]

        [SerializeField]
        private string currencyContentId = "gold";

        [Header("UI")]

        [SerializeField]
        private TMP_Text goldValue;

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (goldValue == null)
            {
                UnityEngine.Debug.LogError(
                    "[PersonalStashCurrencyUI] Gold Value " +
                    "Text is not assigned.");

                return;
            }

            BootstrapContext context =
                BootstrapContext.Current;

            if (context == null ||
                context.PersistentCurrencyState == null)
            {
                UnityEngine.Debug.LogWarning(
                    "[PersonalStashCurrencyUI] Persistent " +
                    "Currency State is not available.");

                goldValue.text = "0";
                return;
            }

            int amount =
                context.PersistentCurrencyState.GetAmount(
                    currencyContentId);

            goldValue.text = amount.ToString("N0");
        }
    }
}