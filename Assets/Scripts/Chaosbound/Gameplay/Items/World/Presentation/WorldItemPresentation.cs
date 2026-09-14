using Chaosbound.Content.Items;
using Chaosbound.Gameplay.Items.Runtime;
using UnityEngine;

namespace Chaosbound.Gameplay.Items.World.Presentation
{
    public sealed class WorldItemPresentation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private WorldItemName itemName;

        [SerializeField]
        private ItemTierPresentationDatabase presentationDatabase;

        private WorldItemTierPresentationFX currentPresentation;

        public void Apply(
            ItemBaseData baseData,
            ItemInstance itemInstance)
        {
            ClearPresentation();

            if (baseData == null)
            {
                return;
            }

            if (itemInstance == null)
            {
                return;
            }

            if (presentationDatabase == null)
            {
                return;
            }

            if (itemName == null)
            {
                return;
            }

            if (!presentationDatabase.TryGet(
                    itemInstance.CurrentTier,
                    out ItemTierPresentationDefinition definition))
            {
                return;
            }

            itemName.SetName(
                baseData.DisplayName);

            itemName.SetColor(
                definition.NameColor);

            SpawnPresentation(
                definition.PresentationPrefab);
        }

        private void SpawnPresentation(
            GameObject presentationPrefab)
        {
            if (presentationPrefab == null)
            {
                return;
            }

            if (PoolManager.Instance == null)
            {
                return;
            }

            WorldItemTierPresentationFX presentation =
                PoolManager.Instance.Get<WorldItemTierPresentationFX>(
                    presentationPrefab,
                    transform.position,
                    Quaternion.identity);

            if (presentation == null)
            {
                return;
            }

            currentPresentation = presentation;

            Transform presentationTransform =
                presentation.transform;

            presentationTransform.SetParent(
                transform,
                false);

            presentationTransform.localPosition =
                Vector3.zero;

            presentationTransform.localRotation =
                Quaternion.identity;

            presentationTransform.localScale =
                Vector3.one;
        }

        private void ClearPresentation()
        {
            if (currentPresentation == null)
            {
                return;
            }

            currentPresentation.ReturnToPool();
            currentPresentation = null;
        }

        public void Clear()
        {
            if (itemName != null)
            {
                itemName.Clear();
            }

            ClearPresentation();
        }
    }
}