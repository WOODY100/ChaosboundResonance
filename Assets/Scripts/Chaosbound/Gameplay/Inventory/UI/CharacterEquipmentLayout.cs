using UnityEngine;

namespace Chaosbound.Gameplay.Inventory.UI
{
    public sealed class CharacterEquipmentLayout : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform mainWeaponSlot;
        [SerializeField] private RectTransform armorSlot;
        [SerializeField] private RectTransform pantsSlot;
        [SerializeField] private RectTransform bootsSlot;
        [SerializeField] private RectTransform helmetSlot;
        [SerializeField] private RectTransform glovesSlot;
        [SerializeField] private RectTransform ringSlot;
        [SerializeField] private RectTransform pendantSlot;
        [SerializeField] private RectTransform specialRelicSlot;

        [Header("Horizontal Positions")]
        [SerializeField] private float leftX = -105f;
        [SerializeField] private float rightX = 105f;

        [Header("Left Side Y Positions")]
        [SerializeField] private float helmetY = 190f;
        [SerializeField] private float armorY = 65f;
        [SerializeField] private float pantsY = -60f;
        [SerializeField] private float bootsY = -185f;
        [SerializeField] private float mainWeaponY = -310f;

        [Header("Right Side Y Positions")]
        [SerializeField] private float pendantY = 190f;
        [SerializeField] private float ringY = 65f;
        [SerializeField] private float glovesY = -60f;
        [SerializeField] private float specialRelicY = -185f;

        private void Awake()
        {
            ApplyLayout();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                ApplyLayout();
            }
        }

        private void ApplyLayout()
        {
            SetPosition(
                helmetSlot,
                leftX,
                helmetY);

            SetPosition(
                armorSlot,
                leftX,
                armorY);

            SetPosition(
                pantsSlot,
                leftX,
                pantsY);

            SetPosition(
                bootsSlot,
                leftX,
                bootsY);

            SetPosition(
                mainWeaponSlot,
                leftX,
                mainWeaponY);

            SetPosition(
                pendantSlot,
                rightX,
                pendantY);

            SetPosition(
                ringSlot,
                rightX,
                ringY);

            SetPosition(
                glovesSlot,
                rightX,
                glovesY);

            SetPosition(
                specialRelicSlot,
                rightX,
                specialRelicY);
        }

        private void SetPosition(
            RectTransform slot,
            float x,
            float y)
        {
            if (slot == null)
                return;

            slot.anchorMin =
                new Vector2(0.5f, 0.5f);

            slot.anchorMax =
                new Vector2(0.5f, 0.5f);

            slot.pivot =
                new Vector2(0.5f, 0.5f);

            slot.anchoredPosition =
                new Vector2(x, y);
        }
    }
}